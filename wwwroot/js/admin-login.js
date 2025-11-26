// /wwwroot/js/admin-login.js
// Maneja el submit del formulario de login sin código embebido en la vista.

(function () {
	const form = document.getElementById('frm-login');
	const btn = document.getElementById('btn-submit');
	const box = document.getElementById('login-error');

	if (!form) return;

	form.addEventListener('submit', async (e) => {
		e.preventDefault();
		box.style.display = 'none';
		box.textContent = '';

		// Validación mínima HTML5
		if (!form.checkValidity()) {
			form.classList.add('was-validated');
			return;
		}

		const formData = new FormData(form);
		btn.disabled = true;

		try {
			const res = await fetch('/Api/Admin/Login', {
				method: 'POST',
				body: formData,
				credentials: 'include',
			});

			if (!res.ok) {
				const txt = await res.text();
				throw new Error(txt || 'Credenciales inválidas.');
			}

			const data = await res.json(); // { token, admin: { ... } }
			localStorage.setItem('adminToken', data.token);
			localStorage.setItem('adminData', JSON.stringify(data.admin));

			// Crear cookie de autenticacion para vistas Razor
			if (data.admin) {
				const formData = new FormData();
				formData.append('nombre', data.admin.NombreUsuario ?? data.admin.nombreUsuario ?? '');
				formData.append('ministerio', (data.admin.IdMinisterio ?? data.admin.idMinisterio ?? '').toString());
				formData.append('id', (data.admin.ID ?? data.admin.id ?? '').toString());

				try {
					await fetch('/Admin/EstablecerCookie', {
						method: 'POST',
						body: formData,
						credentials: 'include',
					});
				} catch (cookieErr) {
					console.error('No se pudo establecer la cookie de sesion.', cookieErr);
				}
			}

			// Redirigir al Dashboard MVC
			window.location.href = '/Admin/Dashboard';
		} catch (err) {
			box.textContent = err.message || 'Error al iniciar sesión.';
			box.style.display = 'block';
		} finally {
			btn.disabled = false;
		}
	});

	// Flujo de recuperación de contraseña
	const recuperar = document.getElementById('btn-recuperar');
	if (recuperar) {
		recuperar.addEventListener('click', async () => {
			if (typeof Swal === 'undefined') {
				console.error('SweetAlert no está disponible.');
				return;
			}

			const { isConfirmed, value: correo } = await Swal.fire({
				title: 'Recuperar acceso',
				text: 'Ingresa el correo asociado a tu usuario de administrador.',
				input: 'email',
				inputPlaceholder: 'correo@ejemplo.com',
				confirmButtonText: 'Enviar correo',
				showCancelButton: true,
				focusConfirm: false,
				preConfirm: async (email) => {
					if (!email) {
						Swal.showValidationMessage('Debes ingresar un correo.');
						return false;
					}
					try {
						const formData = new FormData();
						formData.append('correo', email.trim());

						const res = await fetch('/Api/Admin/ClaveOlvidada', {
							method: 'POST',
							body: formData,
						});

						if (!res.ok) {
							const msg = await res.text();
							throw new Error(msg || 'No se pudo enviar el correo.');
						}

						return email;
					} catch (err) {
						Swal.showValidationMessage(err.message || 'Error al enviar el correo.');
						return false;
					}
				},
			});

			if (isConfirmed && correo) {
				await Swal.fire({
					icon: 'success',
					title: 'Correo enviado',
					text: `Si el correo ${correo} está registrado, recibirás un enlace para restablecer tu contraseña.`,
				});
			}
		});
	}
})();
