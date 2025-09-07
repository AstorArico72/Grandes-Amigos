async function renderDashboard() {
	// 1. Dibuja el esqueleto del HTML del dashboard.
	mainContent.innerHTML = `
        <h1 class="h4 mb-4">Dashboard</h1>
        <div class="row g-3 mb-3">
            <div class="col-12 col-md-6 col-xxl-3">
                <div class="kpi rounded-3 p-3">
                    <div class="small text-secondary">Usuarios Registrados</div>
                    <div class="fs-3 fw-semibold" id="kpiUsuarios">Cargando...</div>
                </div>
            </div>
            <div class="col-12 col-md-6 col-xxl-3">
                <div class="kpi rounded-3 p-3">
                    <div class="small text-secondary">Eventos Activos</div>
                    <div class="fs-3 fw-semibold" id="kpiEventos">Cargando...</div>
                </div>
            </div>
        </div>
        <div class="card">
            <div class="card-header fw-semibold">Próximos eventos</div>
            <div class="table-responsive">
                <table class="table align-middle m-0">
                    <thead><tr><th>Título</th><th>Fecha</th><th></th></tr></thead>
                    <tbody id="tblEventos"><tr><td colspan="3" class="text-center p-4">Cargando...</td></tr></tbody>
                </table>
            </div>
        </div>`;

	// 2. Llama a las APIs para obtener los datos.
	try {
		// Hacemos las dos peticiones en paralelo para más eficiencia.
		const [usersResponse, eventsResponse] = await Promise.all([
			// Petición a un endpoint PROTEGIDO
			fetch('/Api/Usuarios/Todos', { headers: getAuthHeaders() }),
			// Petición a un endpoint PÚBLICO
			fetch('/Api/Eventos/Lista'),
		]);

		// 3. Procesa y muestra los datos de Usuarios.
		const kpiUsuariosEl = document.getElementById('kpiUsuarios');
		if (usersResponse.ok) {
			const users = await usersResponse.json();
			kpiUsuariosEl.textContent = users.length;
		} else {
			// Si la respuesta no es OK (ej. 401 Unauthorized), muestra un error.
			kpiUsuariosEl.textContent = 'Error';
			kpiUsuariosEl.classList.add('text-danger');
		}

		// 4. Procesa y muestra los datos de Eventos.
		const kpiEventosEl = document.getElementById('kpiEventos');
		const eventTableBody = document.getElementById('tblEventos');
		if (eventsResponse.ok) {
			const events = await eventsResponse.json();
			kpiEventosEl.textContent = events.length;

			eventTableBody.innerHTML = ''; // Limpiar la tabla
			if (events.length > 0) {
				events.slice(0, 5).forEach((ev) => {
					// Muestra solo los primeros 5
					eventTableBody.innerHTML += `
                        <tr>
                            <td>${ev.título}</td>
                            <td>${new Date(ev.fecha).toLocaleDateString(
															'es-AR'
														)}</td>
                            <td class="text-end"><a href="#eventos" class="btn btn-sm btn-outline-light">Ver</a></td>
                        </tr>`;
				});
			} else {
				eventTableBody.innerHTML =
					'<tr><td colspan="3" class="text-center p-4">No hay eventos próximos.</td></tr>';
			}
		} else {
			kpiEventosEl.textContent = 'Error';
			kpiEventosEl.classList.add('text-danger');
			eventTableBody.innerHTML =
				'<tr><td colspan="3" class="text-center p-4 text-danger">Error al cargar eventos.</td></tr>';
		}
	} catch (error) {
		console.error('Error al cargar datos del dashboard:', error);
		document.getElementById('kpiUsuarios').textContent = 'Error';
		document.getElementById('kpiEventos').textContent = 'Error';
	}
}
