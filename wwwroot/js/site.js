//------------------------------------------------------------
// ANIMACIONES DE SECCIONES
//------------------------------------------------------------
document.addEventListener('DOMContentLoaded', function () {
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

//------------------------------------------------------------
// OWL CAROUSEL
//------------------------------------------------------------
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

//------------------------------------------------------------
// VALIDACIÓN DE REGISTRO
//------------------------------------------------------------
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

	form.addEventListener('submit', (e) => {
		e.preventDefault();

		let errores = [];
		let primerError = null;

		for (const [campo, input] of Object.entries(inputs)) {
			const valor = input.value.trim();
			const msg = validarCampo(campo, valor, inputs);
			if (msg) {
				errores.push(`<li>${msg}</li>`);
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

		const datos = new FormData(form);
		fetch('/Api/Usuarios/Nuevo', { method: 'POST', body: datos })
			.then((r) => {
				if (!r.ok) throw new Error();
				return r.text();
			})
			.then(() => {
				Swal.fire({
					icon: 'success',
					title: '¡Registro exitoso!',
					text: 'Ya podés iniciar sesión.',
				});
				form.reset();
				document.querySelector('#login-tab').click();
			})
			.catch(() => {
				Swal.fire({
					icon: 'error',
					title: 'No se pudo registrar',
					text: 'Verificá si el DNI ya está registrado.',
				});
			});
	});

	function validarCampo(campo, valor, inputs) {
		switch (campo) {
			case 'Nombre':
				if (!/^[a-zA-ZÁÉÍÓÚáéíóúñÑ\s]{3,}$/.test(valor))
					return 'Nombre inválido.';
				break;
			case 'Correo':
				if (!valor.includes('@')) return 'Correo inválido.';
				break;
			case 'NumDocumento':
				if (!/^\d{6,9}$/.test(valor)) return 'DNI inválido.';
				break;
			case 'Teléfono':
				if (!/^\d{10}$/.test(valor)) return 'Teléfono inválido.';
				break;
			case 'Clave':
				if (valor.length < 6) return 'La contraseña es corta.';
				break;
			case 'RepetirClave':
				if (valor !== inputs.Clave.value.trim())
					return 'Las contraseñas no coinciden.';
				break;
		}
		return null;
	}
});

//------------------------------------------------------------
// MOSTRAR / OCULTAR CONTRASEÑA
//------------------------------------------------------------
document.addEventListener('click', function (e) {
	const toggle = e.target.closest('.toggle-pass');
	if (!toggle) return;

	const input = document.getElementById(toggle.dataset.target);
	const icon = toggle.querySelector('i');

	if (input.type === 'password') {
		input.type = 'text';
		icon.classList.replace('bi-eye', 'bi-eye-slash');
	} else {
		input.type = 'password';
		icon.classList.replace('bi-eye-slash', 'bi-eye');
	}
});

//------------------------------------------------------------
// LOGIN + GUARDADO TOKEN
//------------------------------------------------------------
document.addEventListener('DOMContentLoaded', () => {
	const loginForm = document.querySelector('#login form');
	if (!loginForm) return;

	loginForm.addEventListener('submit', function (e) {
		e.preventDefault();
		const datos = new FormData(loginForm);

		fetch('/Api/Auth/Login', { method: 'POST', body: datos })
			.then(async (resp) => {
				if (!resp.ok) throw new Error(await resp.text());
				const data = await resp.json();

				localStorage.setItem('jwt_token_inscrito', data.token);
				localStorage.setItem(
					'nombre_inscrito',
					data.usuario.Nombre || data.usuario.nombre
				);
				localStorage.setItem(
					'dni_inscrito',
					data.usuario.NumDocumento || data.usuario.numDocumento
				);

				await Swal.fire({
					icon: 'success',
					title: '¡Login exitoso!',
					timer: 2000,
					showConfirmButton: false,
				});

				location.reload();
			})
			.catch((err) => {
				Swal.fire({
					icon: 'error',
					title: 'Error de login',
					text: err.message || 'Usuario o clave incorrectos.',
				});
			});
	});

	const token = localStorage.getItem('jwt_token_inscrito');
	const nombre = localStorage.getItem('nombre_inscrito');

	if (token && nombre) {
		const loginBtn = document.querySelector('[data-bs-target="#authModal"]');
		if (loginBtn) {
			loginBtn.outerHTML = `
                <li class="nav-item dropdown">
                    <a class="nav-link dropdown-toggle header-btn text-white fw-bold rounded-pill px-4"
                       href="#" role="button" data-bs-toggle="dropdown">
                        ${nombre}
                    </a>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item" data-bs-toggle="modal" data-bs-target="#misEventosModal">Mis Eventos</a></li>
                        <li><a class="dropdown-item text-danger" id="cerrarSesionBtn">Cerrar Sesión</a></li>
                    </ul>
                </li>`;
		}
	}

	document.addEventListener('click', (e) => {
		if (e.target.closest('#cerrarSesionBtn')) {
			localStorage.clear();
			location.reload();
		}
	});
});

//------------------------------------------------------------
// MODAL "MIS EVENTOS"
//------------------------------------------------------------
document.addEventListener('DOMContentLoaded', () => {
	const modal = document.getElementById('misEventosModal');
	if (!modal) return;

	modal.addEventListener('show.bs.modal', () => {
		const token = localStorage.getItem('jwt_token_inscrito');
		const cont = document.getElementById('listaMisEventos');

		if (!token) {
			cont.innerHTML = `<div class="alert alert-warning">Debes iniciar sesión.</div>`;
			return;
		}

		cont.innerHTML = `
            <div class="text-center py-4">
                <div class="spinner-border text-primary"></div>
                <p>Cargando tus eventos...</p>
            </div>`;

		fetch('/Api/Eventos/MisEventos', {
			headers: { Authorization: 'Bearer ' + token },
		})
			.then((r) => r.json())
			.then((eventos) => {
				if (!eventos.length) {
					cont.innerHTML = `<div class="alert alert-info">No estás inscrito en ningún evento.</div>`;
					return;
				}

				cont.innerHTML = eventos
					.map(
						(ev) => `
                    <div class="col-md-6 col-lg-4">
                        <div class="card h-100 shadow-sm">
                            <img src="${
															ev.foto
														}" class="card-img-top" style="height:200px;object-fit:cover;">
                            <div class="card-body d-flex flex-column">
                                <h5>${ev.título}</h5>
                                <p class="text-truncate">${
																	ev.descripcion || ''
																}</p>
                                <small class="mt-auto text-muted">
                                    <i class="bi bi-calendar-event"></i>
                                    ${new Date(ev.fecha).toLocaleDateString(
																			'es-AR'
																		)}
                                </small>
                            </div>
                        </div>
                    </div>
                `
					)
					.join('');
			})
			.catch(() => {
				cont.innerHTML = `<div class="alert alert-danger">Error al cargar eventos.</div>`;
			});
	});
});

//------------------------------------------------------------
// *** MINISTERIOS — VERSIÓN CORRECTA (ÚNICA)
//------------------------------------------------------------
(function () {
	const ministeriosContainer = document.getElementById('ministerios-container');
	const modalElement = document.getElementById('modalEventosMinisterio');

	if (!ministeriosContainer) return;

	// MODAL OMITIDO HASTA QUE ME CONFIRMES QUE EXISTE EN ESTA VISTA
	// -------------------------------------------------------------

	function cargarMinisteriosPublicos() {
		fetch('/Api/Ministerios/Publicos')
			.then((r) => r.json())
			.then((ministerios) => {
				if (!ministerios.length) {
					ministeriosContainer.innerHTML = `<div class="col-12 text-center text-muted">No hay ministerios disponibles.</div>`;
					return;
				}

				ministeriosContainer.innerHTML = ministerios
					.map(
						(m) => `
                    <div class="col">
                        <div class="card h-100 shadow-sm">
                            <div class="card-body">
                                <h5 class="card-title">${m.nombre}</h5>
                                <p class="card-text">Actividades y eventos disponibles.</p>
                            </div>
                        </div>
                    </div>
                `
					)
					.join('');
			})
			.catch(() => {
				ministeriosContainer.innerHTML = `<div class="alert alert-danger">Error al cargar ministerios.</div>`;
			});
	}

	cargarMinisteriosPublicos();
})();
