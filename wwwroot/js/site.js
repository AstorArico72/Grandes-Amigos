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
// VALIDACIÓN REGISTRO
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
// LOGIN + TOKEN
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

	// Header dinámico
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
                    </div>`
					)
					.join('');
			})
			.catch(() => {
				cont.innerHTML = `<div class="alert alert-danger">Error al cargar eventos.</div>`;
			});
	});
});

//------------------------------------------------------------
// INSCRIPCIÓN — LÓGICA ORIGINAL COMPLETA
//------------------------------------------------------------
document.addEventListener('DOMContentLoaded', function () {
	let eventoSeleccionadoId = null;

	document.querySelectorAll('.btn-inscribirse').forEach((btn) => {
		btn.addEventListener('click', function () {
			eventoSeleccionadoId = this.getAttribute('data-evento-id');
			const btnConfirmar = document.getElementById('btnConfirmarInscripcion');
			btnConfirmar.disabled = true;
			btnConfirmar.textContent = 'Verificando...';

			document.getElementById('infoEventoModal').innerHTML = `
                <div class="text-center py-3">
                    <div class="spinner-border text-info" role="status"></div>
                </div>`;

			fetch(`/Api/Eventos/${eventoSeleccionadoId}`)
				.then((resp) => resp.json())
				.then((evento) => {
					document.getElementById('infoEventoModal').innerHTML = `
                        <div class="row align-items-center">
                            <div class="col-md-5 text-center mb-3">
                                <img src="${
																	evento.foto
																}" class="img-fluid rounded shadow" style="max-height:220px;object-fit:cover;">
                            </div>
                            <div class="col-md-7">
                                <h4 class="fw-bold mb-2">${evento.título}</h4>
                                <p class="mb-1"><i class="bi bi-calendar-event"></i> <strong>Fecha:</strong>
                                ${new Date(evento.fecha).toLocaleString(
																	'es-AR',
																	{ dateStyle: 'long', timeStyle: 'short' }
																)}</p>
                                <p class="mb-1"><i class="bi bi-people"></i> <strong>Ministerio:</strong> ${
																	evento.ministerioNombre ||
																	evento.id_Ministerio
																}</p>
                                <p><i class="bi bi-info-circle"></i> <strong>Descripción:</strong> ${
																	evento.descripcion || ''
																}</p>
                            </div>
                        </div>`;

					const token = localStorage.getItem('jwt_token_inscrito');
					if (!token) {
						btnConfirmar.disabled = true;
						btnConfirmar.textContent = 'Iniciá sesión para inscribirte';
						return;
					}

					function parseJwt(t) {
						try {
							return JSON.parse(atob(t.split('.')[1]));
						} catch {
							return null;
						}
					}

					const payload = parseJwt(token);
					const userId =
						payload && (payload.NumDocumento || payload.numDocumento);

					if (!userId) {
						btnConfirmar.disabled = true;
						btnConfirmar.textContent = 'Error de usuario';
						return;
					}

					fetch(
						`/Api/Inscripciones/Existe?eventoId=${eventoSeleccionadoId}&usuarioId=${userId}`,
						{
							headers: { Authorization: 'Bearer ' + token },
						}
					)
						.then((resp) => resp.json())
						.then((data) => {
							if (data.inscripto) {
								btnConfirmar.disabled = true;
								btnConfirmar.textContent = 'Ya inscrito';
							} else {
								btnConfirmar.disabled = false;
								btnConfirmar.textContent = 'Inscribirse';
							}
						})
						.catch(() => {
							btnConfirmar.disabled = true;
							btnConfirmar.textContent = 'Error al verificar';
						});
				})
				.catch(() => {
					document.getElementById(
						'infoEventoModal'
					).innerHTML = `<div class="alert alert-danger">No se pudo cargar la información del evento.</div>`;
					btnConfirmar.disabled = true;
					btnConfirmar.textContent = 'Error';
				});
		});
	});

	document
		.getElementById('btnConfirmarInscripcion')
		.addEventListener('click', function () {
			const btn = this;
			if (btn.disabled) return;

			const token = localStorage.getItem('jwt_token_inscrito');
			if (!token) {
				Swal.fire('Debes iniciar sesión para inscribirte.', '', 'warning');
				return;
			}

			function parseJwt(t) {
				try {
					return JSON.parse(atob(t.split('.')[1]));
				} catch {
					return null;
				}
			}

			const payload = parseJwt(token);
			const userId = payload && (payload.NumDocumento || payload.numDocumento);

			if (!userId) {
				Swal.fire('Error obteniendo usuario', '', 'error');
				return;
			}

			fetch('/Api/Inscripciones', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
					Authorization: 'Bearer ' + token,
				},
				body: JSON.stringify({
					id_Evento: eventoSeleccionadoId,
					id_Inscrito: userId,
				}),
			}).then((resp) => {
				if (resp.ok) {
					Swal.fire('¡Inscripción exitosa!', '', 'success');
					bootstrap.Modal.getInstance(
						document.getElementById('inscripcionModal')
					).hide();
				} else {
					resp.text().then((text) => {
						let msg = 'Error al inscribirse';
						try {
							const d = JSON.parse(text);
							msg = d.message || msg;
						} catch {}
						Swal.fire('Error', msg, 'error');
					});
				}
			});
		});
});

//------------------------------------------------------------
// MINISTERIOS + MODAL
//------------------------------------------------------------
(function () {
	const ministeriosContainer = document.getElementById('ministerios-container');
	const modalElement = document.getElementById('modalEventosMinisterio');
	if (!ministeriosContainer || !modalElement) return;

	const modal = new bootstrap.Modal(modalElement);
	const modalTitulo = document.getElementById('modalEventosMinisterioLabel');
	const modalSpinner = document.getElementById('modalEventosSpinner');
	const modalContenido = document.getElementById('modalEventosContenido');
	const modalAlert = document.getElementById('modalEventosAlert');

	function renderEventosMinisterio(eventos) {
		modalSpinner.classList.add('d-none');
		modalAlert.classList.add('d-none');
		modalContenido.innerHTML = '';

		if (!eventos || eventos.length === 0) {
			modalContenido.innerHTML =
				'<div class="col-12 text-center text-muted">No hay eventos próximos</div>';
			return;
		}

		eventos.forEach((evento) => {
			const fechaEvento = evento.fecha ? new Date(evento.fecha) : null;
			const fechaFormateada = fechaEvento
				? fechaEvento.toLocaleString('es-AR', {
						dateStyle: 'long',
						timeStyle: 'short',
				  })
				: 'Fecha no disponible';

			const card = document.createElement('div');
			card.className = 'col-12 col-md-6';

			card.innerHTML = `
                <div class="card h-100 shadow-sm border-0">
                    ${
											evento.foto
												? `<img src="${evento.foto}" class="card-img-top" style="height:180px;object-fit:cover;">`
												: ''
										}
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title">${
													evento.título || 'Evento'
												}</h5>
                        <p class="text-muted mb-2"><i class="bi bi-calendar-event"></i> ${fechaFormateada}</p>
                        <p class="card-text">${
													evento.descripción || 'Sin descripción'
												}</p>
                    </div>
                </div>`;

			modalContenido.appendChild(card);
		});
	}

	function abrirModalMinisterio(nombreMinisterio) {
		modalTitulo.textContent = `Eventos de ${nombreMinisterio}`;
		modalContenido.innerHTML = '';
		modalAlert.classList.add('d-none');
		modalSpinner.classList.remove('d-none');
		modal.show();

		fetch(`/Api/Eventos/PorMinisterio/${encodeURIComponent(nombreMinisterio)}`)
			.then((r) => {
				if (!r.ok) {
					throw new Error('No se pudieron obtener los eventos.');
				} else {
					return r.json();
				}
			})
			.then((eventos) => {
				const ahora = new Date();
				const futuros = eventos.filter((ev) => new Date(ev.fecha) > ahora);
				renderEventosMinisterio(futuros);
			})
			.catch((err) => {
				modalSpinner.classList.add('d-none');
				modalAlert.textContent = err.message;
				modalAlert.classList.remove('d-none');
			});
	}

	function cargarMinisteriosPublicos() {
		fetch('/Api/Ministerios/Publicos')
			.then((r) => r.json())
			.then((ministerios) => {
				ministeriosContainer.innerHTML = ministerios
					.map(
						(m) => `
                    <div class="col">
                        <div class="card h-100 shadow-sm border-0">
                            <div class="card-body d-flex flex-column">
                                <h5 class="card-title">${m.nombre}</h5>
                                <p class="card-text">Actividades y eventos disponibles.</p>

                                <div class="mt-auto">
                                    <button class="btn btn-outline-primary btn-ver-mas"
                                            data-ministerio="${m.nombre}">
                                        Ver más
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>`
					)
					.join('');

				ministeriosContainer.querySelectorAll('.btn-ver-mas').forEach((btn) => {
					btn.addEventListener('click', () => {
						abrirModalMinisterio(btn.dataset.ministerio);
					});
				});
			})
			.catch(() => {
				ministeriosContainer.innerHTML = `<div class="alert alert-danger">Error al cargar ministerios.</div>`;
			});
	}

	cargarMinisteriosPublicos();
})();
