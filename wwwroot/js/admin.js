(function () {
    'use strict';

    function getAuthHeaders() {
        const t = localStorage.getItem('adminToken');
        return t ? { Authorization: `Bearer ${t}` } : {};
    }

    function getAdminName() {
        const raw = localStorage.getItem('adminData');
        if (!raw) return null;
        try {
            const data = JSON.parse(raw);
            return (
                data.NombreUsuario ||
                data.nombreUsuario ||
                data.Nombre ||
                data.name ||
                null
            );
        } catch {
            return null;
        }
    }

    // Exponer para que otros scripts (usuarios.js, etc.) lo usen
    window.getAuthHeaders = getAuthHeaders;

    // ====== Dashboard: hidratar KPIs y tabla si existen ======
    async function hydrateDashboard() {
        const kpiUsuariosEl = document.getElementById('kpiUsuarios');
        const kpiEventosEl = document.getElementById('kpiEventos');
        const eventTableBody = document.getElementById('tblEventos');

        // Si no hay ninguno de estos elementos, no estamos en el dashboard
        if (!kpiUsuariosEl && !kpiEventosEl && !eventTableBody) return;

        try {
            // Pedidos en paralelo (usuarios requiere Bearer; eventos es publico)
            const [usersRes, eventsRes] = await Promise.all([
                fetch('/Api/Usuarios/Todos', { headers: getAuthHeaders() }),
                fetch('/Api/Eventos/Lista'),
            ]);

            // Usuarios
            if (kpiUsuariosEl) {
                if (usersRes.ok) {
                    const users = await usersRes.json();
                    kpiUsuariosEl.textContent = Array.isArray(users) ? users.length : '0';
                } else {
                    kpiUsuariosEl.textContent = 'Error';
                    kpiUsuariosEl.classList.add('text-danger');
                }
            }

            // Eventos
            if (kpiEventosEl || eventTableBody) {
                if (eventsRes.ok) {
                    const events = await eventsRes.json();
                    if (kpiEventosEl)
                        kpiEventosEl.textContent = Array.isArray(events)
                            ? events.length
                            : '0';

                    if (eventTableBody) {
                        eventTableBody.innerHTML = '';
                        const list = Array.from (events);
                        if (list.length === 0) {
                            eventTableBody.innerHTML =
                                '<tr><td colspan="3" class="text-center p-4">No hay eventos proximos.</td></tr>';
                        } else {
                            for (const ev of list) {
                                const fecha = ev.fecha ? new Date(ev.fecha) : null;
                                const fechaStr = fecha
                                    ? fecha.toLocaleDateString('es-AR', {
                                          day: '2-digit',
                                          month: '2-digit',
                                          year: 'numeric',
                                          hour: "numeric",
                                          hour12: false,
                                          minute: "numeric"
                                      })
                                    : '-';
                                eventTableBody.insertAdjacentHTML(
                                    'beforeend',
                                    `<td class="fw-semibold">${ev.título ?? '(sin título)'}</td>
                                    <td>${fechaStr} hs</td>
                                    <td><span class="badge bg-dark-subtle text-light">${
			                        					ev.ID_Ministerio ?? ev.id_ministerio ?? '-'
			                        				}</span></td>
                                    <td class="text-end">
                                      <a class="btn btn-sm btn-outline-light" href="/Admin/EditarEvento/${
			                        						ev.ID ?? ev.id
			                        					}">
                                        <i class="bi bi-pencil"></i>
                                      </a>
                                    </td>`
                                );
                            }
                        }
                    }
                } else {
                    if (kpiEventosEl) {
                        kpiEventosEl.textContent = 'Error';
                        kpiEventosEl.classList.add('text-danger');
                    }
                    if (eventTableBody) {
                        eventTableBody.innerHTML =
                            '<tr><td colspan="3" class="text-center p-4 text-danger">Error al cargar eventos.</td></tr>';
                    }
                }
            }
        } catch (err) {
            console.error('Error al cargar datos del dashboard:', err);
            if (kpiUsuariosEl) kpiUsuariosEl.textContent = 'Error';
            if (kpiEventosEl) kpiEventosEl.textContent = 'Error';
        }
    }

    function hydrateSessionName() {
        const name = getAdminName();
        if (!name) return;
        document.querySelectorAll('#kpiSesion').forEach((el) => {
            el.textContent = name;
        });
    }

    // ====== Logout (JWT-only) opcional ======
	function wireLogout() {
		const btn = document.getElementById('btnLogoutJwt');
		if (!btn) return;
		btn.addEventListener('click', async (e) => {
			e.preventDefault();
			localStorage.removeItem('adminToken');
			try {
				await fetch('/Admin/Logout', {
					method: 'GET',
					credentials: 'include',
				});
			} catch (err) {
				console.error('Error al cerrar sesion', err);
			}
			window.location.replace('/Admin/Login');
		});
	}

	// ====== Boot ======
	document.addEventListener('DOMContentLoaded', () => {
        hydrateDashboard();
        hydrateSessionName();
        wireLogout();
    });
})();
