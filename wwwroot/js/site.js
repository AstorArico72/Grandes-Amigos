document.addEventListener('DOMContentLoaded', function () {
	// Animaciones de entrada de secciones
	const secciones = document.querySelectorAll('.seccion-animada');
	const observer = new IntersectionObserver(
		(entries) => {
			entries.forEach((entry) => {
				if (entry.isIntersecting) {
					entry.target.classList.add('seccion-visible');
				}
			});
		},
		{ threshold: 0.2 }
	);
	secciones.forEach((seccion) => observer.observe(seccion));
});

// OwlCarousel
$(document).ready(function () {
	$('.carousel-noticias').owlCarousel({
		loop: true,
		margin: 16,
		nav: true,
		dots: false,
		navText: [
			'<i class="bi bi-chevron-left fs-3 text-white bg-primary rounded-circle px-2 py-1"></i>',
			'<i class="bi bi-chevron-right fs-3 text-white bg-primary rounded-circle px-2 py-1"></i>',
		],
		responsive: {
			0: { items: 1 },
			768: { items: 3 },
		},
	});
});

// Validación del registro
document.addEventListener('DOMContentLoaded', () => {
	const form = document.querySelector('#register form');
	if (!form) return;

	const inputs = {
		Nombre: form.querySelector("[name='Nombre']"),
		Correo: form.querySelector("[name='Correo']"),
		NumDocumento: form.querySelector("[name='NumDocumento']"),
		Teléfono: form.querySelector("[name='Teléfono']"),
		Clave: form.querySelector("[name='Clave']"),
		RepetirClave: form.querySelector("[name='RepetirClave']"),
	};

	// Validar al salir del campo
	Object.entries(inputs).forEach(([campo, input]) => {
		input.addEventListener('blur', () => {
			const mensaje = validarCampo(campo, input.value.trim(), inputs);
			if (mensaje) {
				Swal.fire({
					icon: 'warning',
					title: `Campo ${campo}`,
					text: mensaje,
				}).then(() => input.focus());
			}
		});
	});

	// Validación al enviar
	form.addEventListener('submit', function (e) {
		e.preventDefault();

		let errores = [];
		let primerError = null;

		for (const [campo, input] of Object.entries(inputs)) {
			const valor = input.value.trim();
			const mensaje = validarCampo(campo, valor, inputs);
			if (mensaje) {
				errores.push(`<li>${mensaje}</li>`);
				if (!primerError) primerError = input;
			}
		}

		if (errores.length > 0) {
			Swal.fire({
				icon: 'error',
				title: 'Revisá los campos',
				html: `<ul style="text-align:left;">${errores.join('')}</ul>`,
			}).then(() => primerError.focus());
			return;
		}

		// Si todo está OK, enviar por fetch
		const datos = new FormData(form);
		fetch('/Api/Inscritos/Nuevo', {
			method: 'POST',
			body: datos,
		})
			.then((resp) => {
				if (!resp.ok) throw new Error('Registro fallido');
				return resp.text();
			})
			.then(() => {
				Swal.fire({
					icon: 'success',
					title: '¡Registro exitoso!',
					text: 'Ya podés iniciar sesión con tu documento y clave.',
				});
				form.reset();
				document.querySelector('#login-tab').click();
			})
			.catch(() => {
				Swal.fire({
					icon: 'error',
					title: 'No se pudo registrar',
					text: 'Verificá si el documento ya está registrado.',
				});
			});
	});

	// Validación campo por campo
	function validarCampo(campo, valor, inputs) {
		switch (campo) {
			case 'Nombre':
				if (!/^[a-zA-ZÁÉÍÓÚáéíóúñÑÄËÏÖÜäëïöüØøẞß\s]{3,}$/.test(valor))
				//Hay más diacríticos además de los acentos. Pendiente: Mejorar la validación de éste campo para que nombres extranjeros con diacríticos, por ejemplo, Hämäläinen (Finlandés), Groß (Alemán), Øster (Danés), Itō (Japonés), puedan procesarse bien sin muchos pasos extra.
					return 'El nombre debe tener al menos 3 letras y solo letras.';
				break;
			case 'Correo':
				if (!valor.includes('@') || valor.length < 5) return 'Correo inválido.';
				break;
			case 'NumDocumento':
				if (!/^\d{6,9}$/.test(valor)) //Creo que aún existe gente con LC o LE de 6 dígitos
					return 'DNI inválido: debe contener solo números (6 a 9 cifras).';
				break;
			case 'Teléfono':
				if (!/^\+54\s?9\s?\d{2,4}\s?\d{3,4}\s?\d{3,4}$/.test(valor))
					return 'Teléfono inválido. Usá formato +54 9 266 123 456.';
				break;
			case 'Clave':
				if (valor.length < 6)
					return 'La contraseña debe tener al menos 6 caracteres.';
				break;
			case 'RepetirClave':
				const original = inputs.Clave.value.trim();
				if (valor !== original) return 'Las contraseñas no coinciden.';
				break;
		}
		return null;
	}
});

// Mostrar/ocultar contraseña (ojito)
document.addEventListener('click', function (e) {
	const toggle = e.target.closest('.toggle-pass');
	if (toggle) {
		const inputId = toggle.dataset.target;
		const input = document.getElementById(inputId);
		const icon = toggle.querySelector('i');

		if (input.type === 'password') {
			input.type = 'text';
			icon.classList.remove('bi-eye');
			icon.classList.add('bi-eye-slash');
		} else {
			input.type = 'password';
			icon.classList.remove('bi-eye-slash');
			icon.classList.add('bi-eye');
		}
	}
});

// Login con Swal
document.addEventListener('DOMContentLoaded', () => {
	const loginForm = document.querySelector('#login form');
	if (!loginForm) return;

	loginForm.addEventListener('submit', function (e) {
		e.preventDefault();

		const datos = new FormData(loginForm);

		fetch('/Api/Inscritos/Login', {
			method: 'POST',
			body: datos,
		})
			.then(async (resp) => {
				if (resp.ok) {
					await Swal.fire({
						icon: 'success',
						title: '¡Login exitoso!',
						text: 'Sesión iniciada correctamente.',
					});
					// Redirigir o recargar página si es necesario
					location.reload();
				} else {
					const msg = await resp.text();
					throw new Error(msg || 'Login fallido');
				}
			})
			.catch((err) => {
				Swal.fire({
					icon: 'error',
					title: 'Error de login',
					text: err.message || 'Usuario o clave incorrectos.',
				});
			});
	});
});

// Guardar token al iniciar sesión y mostrar nombre en el header
document.addEventListener('DOMContentLoaded', () => {
	const loginForm = document.querySelector('#login form');
	if (!loginForm) return;

	loginForm.addEventListener('submit', function (e) {
		e.preventDefault();

		const datos = new FormData(loginForm);

		fetch('/Api/Inscritos/Login', {
			method: 'POST',
			body: datos,
		})
			.then(async (resp) => {
				if (resp.ok) {
					const data = await resp.json();
					// Verifica cómo llega el objeto
					console.log(data.usuario);
					// Guardar token y datos públicos del usuario (soporta mayúscula y minúscula)
					localStorage.setItem('jwt_token_inscrito', data.token);
					localStorage.setItem(
						'nombre_inscrito',
						data.usuario.Nombre || data.usuario.nombre
					);
					localStorage.setItem(
						'dni_inscrito',
						data.usuario.NumDocumento ||
							data.usuario.numDocumento ||
							data.usuario.numdocumento
					);
					await Swal.fire({
						icon: 'success',
						title: '¡Login exitoso!',
						text: 'Sesión iniciada correctamente.',
						timer: 2000, // ⏱️ visible 2 segundos
						showConfirmButton: false,
					});

					setTimeout(() => {
						location.reload();
					}, 2000);
				} else {
					const msg = await resp.text();
					throw new Error(msg || 'Login fallido');
				}
			})
			.catch((err) => {
				Swal.fire({
					icon: 'error',
					title: 'Error de login',
					text: err.message || 'Usuario o clave incorrectos.',
				});
			});
	});

	// Mostrar header dinámico si ya hay token
	const token = localStorage.getItem('jwt_token_inscrito');
	const nombre = localStorage.getItem('nombre_inscrito');
	if (token && nombre) {
		const loginBtn = document.querySelector('[data-bs-target="#authModal"]');
		if (loginBtn) {
			loginBtn.outerHTML = `
				<li class="nav-item dropdown">
					<a class="nav-link dropdown-toggle header-btn text-white fw-bold rounded-pill px-4"
					href="#" role="button" data-bs-toggle="dropdown" aria-expanded="false">
						${nombre}
					</a>
					<ul class="dropdown-menu dropdown-menu-end">
						<li><a class="dropdown-item" href="/Inscripcion/MisEventos">Mis Eventos</a></li>
						<li><a class="dropdown-item text-danger" id="cerrarSesionBtn" href="#">Cerrar Sesión</a></li>
					</ul>
				</li>`;
		}
	}

	// Botón para cerrar sesión
	document.addEventListener('click', (e) => {
		if (e.target.closest('#cerrarSesionBtn')) {
			localStorage.removeItem('jwt_token_inscrito');
			localStorage.removeItem('nombre_inscrito');
			localStorage.removeItem('dni_inscrito');
			location.reload();
		}
	});
});
