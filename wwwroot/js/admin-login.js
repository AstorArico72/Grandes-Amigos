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
			console.log (data);
			localStorage.setItem('adminToken', data.token);
			let admin = JSON.stringify (data.admin)
			console.log ("Admin:" + admin);
			localStorage.setItem('adminData', admin);

			// Redirigir al Dashboard MVC
			window.location.href = '/Admin/Dashboard';
		} catch (err) {
			box.textContent = err.message || 'Error al iniciar sesión.';
			box.style.display = 'block';
		} finally {
			btn.disabled = false;
		}
	});

	// Por ahora, el botón "recuperar" no hace nada (sin acción)
	const recuperar = document.getElementById('btn-recuperar');
	if (recuperar) {
		recuperar.addEventListener('click', () => {
			// Placeholder: acá luego abrís un modal o navegas a /Admin/Recuperar
			// (no implementado por pedido)
		});
	}
})();
