// Contenido para /wwwroot/js/dashboard.js

// Usamos un evento que se dispara cuando el HTML está listo.
document.addEventListener('DOMContentLoaded', function () {
	// --- Helpers para obtener datos de sesión ---
	const token = localStorage.getItem('adminToken');
	const authHeaders = token ? { Authorization: 'Bearer ' + token } : {};
	const noCacheHeaders = {
        'Cache-Control': 'no-cache, no-store, must-revalidate',
        'Pragma': 'no-cache',
        'Expires': '0'
    };
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
		fetch('/Api/Eventos/Lista', {headers: noCacheHeaders})
			.then((r) => {
				if (r.ok) {
					return r.json();
				} else {
					return Promise.reject('Error al cargar eventos');
					}
				}
			)
			.then((list) => {
			//const arr = Array.isArray(list) ? list : [];
			//const arr = Array.from (list);
			kpiEventos.textContent = list.length;
			// Limpiar y renderizar tabla
			tblEventosBody.innerHTML = '';
			if (list.length === 0) {
				tblEventosBody.innerHTML =
					'<tr><td colspan="4" class="text-center py-4 text-secondary">Sin eventos próximos</td></tr>';
				return;
			}
			list
				.sort((a, b) => new Date(a.fecha) - new Date(b.fecha))
				.forEach((ev) => {
					const tr = document.createElement('tr');
					const fechaFormateada = new Date(ev.Fecha).toLocaleString('es-AR', {
						dateStyle: 'short',
						timeStyle: 'short',
					});
					tr.innerHTML = `
    	    <td class="fw-semibold">${ev.Título ?? '(sin título)'}</td>
    	    <td>${fechaFormateada} hs</td>
    	    <td><span class="badge bg-dark-subtle text-light">${
								ev.ID_Ministerio ?? ev.ID_ministerio ?? '-'
							}</span></td>
    	    <td class="text-end">
    	      <button class="btn btn-danger btn-sm btn-outline-light btn-borrar" data-id="${ev.ID}" data-nombre="${ev.Título}"/>
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

	tblEventosBody.addEventListener ('click', async (ev) => {
		const btn = ev.target.closest('.btn-borrar');
		if (!btn) return;

		const id = btn.dataset.id;
		const nombre = btn.dataset.nombre || '';

		const ok = await Swal.fire({
			title: '¿Borrar evento?',
			text: `El evento \"${nombre}\" será borrado, y no puede deshacerse.`,
			icon: 'warning',
			showCancelButton: true,
			confirmButtonText: 'Borrar',
			cancelButtonText: 'Cancelar',
		}).then((r) => r.isConfirmed);

		if (!ok) return;

		try {
			const res = await fetch(
				`/Api/Eventos/Borrar/${encodeURIComponent(id)}`,
				{
					method: 'DELETE',
					headers: authHeaders,
				}
			);
			if (!res.ok) {
				const txt = await res.text();
				throw new Error(txt || 'No se pudo eliminar.');
			}
			Swal.fire('Eliminado', 'El evento fue eliminado.', 'success').then(() => {
				window.location.href = '/Admin/Dashboard';
			});
		} catch (err) {
			Swal.fire('Error', err.message || 'Fallo al eliminar.', 'error');
		}
	});
});
