// Contenido para /wwwroot/js/dashboard.js

// Usamos un evento que se dispara cuando el HTML está listo.
document.addEventListener('DOMContentLoaded', function () {
	// --- Helpers para obtener datos de sesión ---
	const token = localStorage.getItem('adminToken');
	const authHeaders = token ? { Authorization: 'Bearer ' + token } : {};
	const getAdminData = () => {
		try {
			return JSON.parse(localStorage.getItem('adminData') || 'null');
		} catch {
			return null;
		}
	};

	// --- Elementos del DOM ---
	const kpiUsuarios = document.getElementById('kpiUsuarios');
	const kpiEventos = document.getElementById('kpiEventos');
	const kpiNoticias = document.getElementById('kpiNoticias');
	const kpiSesion = document.getElementById('kpiSesion');
	const btnCargarNoticias = document.getElementById('btnCargarNoticias');
	const tblEventosBody = document.getElementById('tblEventos');

	// --- Lógica de la página ---

	// Actualizar KPI de sesión
	const admin = getAdminData();
	if (kpiSesion) {
		kpiSesion.textContent = admin?.nombreUsuario ?? '—';
	}

	// Cargar KPI de Usuarios
	if (kpiUsuarios) {
		fetch('/Api/Usuarios/Todos', { headers: authHeaders })
			.then((r) =>
				r.ok ? r.json() : Promise.reject('Error al cargar usuarios')
			)
			.then((list) => {
				kpiUsuarios.textContent = Array.isArray(list) ? list.length : '0';
			})
			.catch(() => {
				kpiUsuarios.textContent = 'Error';
			});
	}

	// Cargar KPI y tabla de Eventos
	if (kpiEventos && tblEventosBody) {
		fetch('/Api/Eventos/Lista')
			.then((r) =>
				r.ok ? r.json() : Promise.reject('Error al cargar eventos')
			)
			.then((list) => {
				const arr = Array.isArray(list) ? list : [];
				kpiEventos.textContent = arr.length;

				// Limpiar y renderizar tabla
				tblEventosBody.innerHTML = '';
				if (arr.length === 0) {
					tblEventosBody.innerHTML =
						'<tr><td colspan="4" class="text-center py-4 text-secondary">Sin eventos próximos</td></tr>';
					return;
				}

				arr
					.sort((a, b) => new Date(a.fecha) - new Date(b.fecha))
					.slice(0, 8) // Mostrar solo los primeros 8
					.forEach((ev) => {
						const tr = document.createElement('tr');
						const fechaFormateada = new Date(ev.fecha).toLocaleString('es-AR', {
							dateStyle: 'short',
							timeStyle: 'short',
						});

						tr.innerHTML = `
                <td class="fw-semibold">${ev.titulo ?? '(sin título)'}</td>
                <td>${fechaFormateada} hs</td>
                <td><span class="badge bg-dark-subtle text-light">${
									ev.id_Ministerio ?? ev.id_ministerio ?? '-'
								}</span></td>
                <td class="text-end">
                  <a class="btn btn-sm btn-outline-light" href="/Admin/Eventos/Editar/${
										ev.id
									}">
                    <i class="bi bi-pencil"></i>
                  </a>
                </td>`;
						tblEventosBody.appendChild(tr);
					});
			})
			.catch(() => {
				kpiEventos.textContent = 'Error';
				tblEventosBody.innerHTML =
					'<tr><td colspan="4" class="text-center py-4 text-danger">No se pudieron cargar los eventos</td></tr>';
			});
	}

	// Cargar Noticias (acción de botón)
	if (kpiNoticias) kpiNoticias.textContent = 'N/A';

	if (btnCargarNoticias) {
		btnCargarNoticias.addEventListener('click', async (e) => {
			e.preventDefault();
			const btn = e.currentTarget;
			const oldHtml = btn.innerHTML;
			btn.disabled = true;
			btn.innerHTML =
				'<span class="spinner-border spinner-border-sm me-1"></span> Cargando…';

			try {
				const res = await fetch('/Api/Noticias/Cargar', { method: 'POST' }); // Usar POST para acciones
				if (!res.ok) throw new Error('No se pudo cargar el feed RSS');
				Swal.fire({
					icon: 'success',
					title: 'Operación realizada',
					text: 'El cambio se guardó correctamente',
				});
			} catch (err) {
				Swal.fire({
					icon: 'error',
					title: 'Error',
					text: 'No se pudo completar la acción',
				});
			} finally {
				btn.disabled = false;
				btn.innerHTML = oldHtml;
			}
		});
	}
});
