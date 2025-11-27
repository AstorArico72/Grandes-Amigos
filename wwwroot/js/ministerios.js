(function () {
	'use strict';

	console.log('[Ministerios] JS cargado');

	const tablaBody = document.getElementById('tabla-ministerios-body');
	const btnNuevo = document.getElementById('btnNuevoMinisterio');
	const btnRefrescar = document.getElementById('btnRefrescarMinisterios');
	const btnEditar = document.getElementById('btnEditarMinisterio');
	const flags = document.getElementById('ministerios-flags');
	const authHeaders = (window.getAuthHeaders && window.getAuthHeaders()) || {};
	const storage = window.sessionStorage;

	function ensureSwalTheme() {
		if (document.getElementById('swal-admin-styles')) return;
		const style = document.createElement('style');
		style.id = 'swal-admin-styles';
		style.textContent = `
      .swal2-input, .swal2-textarea {
        background-color: #0d1117 !important;
        color: #ffffff !important;
        border: 1px solid #3b82f6 !important;
      }
      .swal2-input::placeholder, .swal2-textarea::placeholder {
        color: #b7b7b7 !important;
      }
    `;
		document.head.appendChild(style);
	}

	function handleException(error) {
		console.error('[Ministerios] Excepción:', error);
		Swal.fire({
			icon: 'error',
			title: 'Excepción JS',
			html: `<pre>${error}</pre>`,
		});
	}

	async function apiFetch(url, options = {}) {
		console.log('[Ministerios] Enviando petición:', url, options);
		const response = await fetch(url, { credentials: 'include', ...options });
		console.log('[Ministerios] Estado de respuesta:', response.status);
		console.log('[Ministerios] Headers:', [...response.headers.entries()]);

		if (!response.ok) {
			const errorText = await response.text();
			console.error('[Ministerios] Error body:', errorText);
			await Swal.fire({
				icon: 'error',
				title: 'Error en API',
				html: `<b>Status:</b> ${response.status}<br><b>Detalle:</b><br><pre>${errorText}</pre>`,
			});
			throw new Error(errorText || `HTTP ${response.status}`);
		}

		return response;
	}

	function renderError(msg) {
		if (!tablaBody) return;
		tablaBody.innerHTML = `<tr><td colspan="3" class="text-center text-danger py-4">${msg}</td></tr>`;
	}

	async function cargarMinisterios() {
		if (!tablaBody) return;
		tablaBody.innerHTML =
			'<tr><td colspan="3" class="text-center py-4 text-secondary">Cargando...</td></tr>';

		try {
			const res = await apiFetch('/Api/Ministerios/Lista', { headers: authHeaders });
			const data = await res.json();

			if (!Array.isArray(data) || data.length === 0) {
				tablaBody.innerHTML =
					'<tr><td colspan="3" class="text-center py-4 text-secondary">No hay ministerios cargados.</td></tr>';
				return;
			}

			tablaBody.innerHTML = '';
			data.forEach((m) => {
				const tr = document.createElement('tr');
				tr.innerHTML = `
          <td>${m.id ?? m.ID ?? '-'}</td>
          <td>${m.nombre ?? m.Nombre ?? ''}</td>
          <td class="text-end d-flex justify-content-end gap-2">
            <button class="btn btn-sm btn-outline-primary" data-accion="editar" data-id="${m.id ?? m.ID}">
              <i class="bi bi-pencil"></i> Editar
            </button>
            <button class="btn btn-sm btn-outline-danger" data-accion="eliminar" data-id="${m.id ?? m.ID}">
              <i class="bi bi-trash"></i> Eliminar
            </button>
          </td>`;
				tablaBody.appendChild(tr);
			});
		} catch (err) {
			handleException(err);
			renderError(err.message || 'No se pudo cargar la lista.');
		}
	}

	function abrirFormulario(titulo, nombreInicial = '') {
		ensureSwalTheme();
		return Swal.fire({
			title: titulo,
			html: `
        <input id="swal-nombre" class="form-control mb-3 swal2-input" placeholder="Nombre" value="${nombreInicial}">
      `,
			showCancelButton: true,
			confirmButtonText: 'Guardar',
			focusConfirm: false,
			preConfirm: () => {
				const nombreInput = document.getElementById('swal-nombre');
				const nombreVal = (nombreInput?.value || '').trim();
				if (!nombreVal) {
					Swal.showValidationMessage('El nombre es obligatorio.');
					return false;
				}
				return { nombre: nombreVal };
			},
		});
	}

	async function abrirCrearMinisterio() {
		const result = await abrirFormulario('Nuevo ministerio');
		if (!result.isConfirmed || !result.value) return;

		try {
			await apiFetch('/Api/Ministerios/Crear', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json', ...authHeaders },
				body: JSON.stringify(result.value),
			});

			await Swal.fire({
				icon: 'success',
				title: 'Operación realizada',
				text: 'El cambio se guardó correctamente',
			});
			await cargarMinisterios();
		} catch (err) {
			handleException(err);
		}
	}

	async function abrirEditarMinisterio(id) {
		if (!id) return;
		try {
			const res = await apiFetch(`/Api/Ministerios/${id}`, {
				headers: authHeaders,
			});
			const data = await res.json();

			const result = await abrirFormulario('Editar ministerio', data.nombre ?? data.Nombre ?? '');
			if (!result.isConfirmed || !result.value) return;

			await apiFetch(`/Api/Ministerios/Editar/${id}`, {
				method: 'PUT',
				headers: { 'Content-Type': 'application/json', ...authHeaders },
				body: JSON.stringify(result.value),
			});

			await Swal.fire({
				icon: 'success',
				title: 'Operación realizada',
				text: 'El cambio se guardó correctamente',
			});
			await cargarMinisterios();
		} catch (err) {
			handleException(err);
		}
	}

	function eliminarMinisterio(id) {
		if (!id) return;
		Swal.fire({
			title: '¿Eliminar?',
			text: 'Esta acción no se puede deshacer',
			icon: 'warning',
			showCancelButton: true,
			confirmButtonText: 'Sí, eliminar',
			cancelButtonText: 'Cancelar',
		}).then(async (r) => {
			if (!r.isConfirmed) return;
			try {
				await apiFetch(`/Api/Ministerios/Eliminar/${id}`, {
					method: 'DELETE',
					headers: authHeaders,
				});

				await Swal.fire({
					icon: 'success',
					title: 'Operación realizada',
					text: 'El cambio se guardó correctamente',
				});
				await cargarMinisterios();
			} catch (err) {
				handleException(err);
			}
		});
	}

	document.addEventListener('DOMContentLoaded', () => {
		if (tablaBody) cargarMinisterios();

		btnNuevo?.addEventListener('click', (e) => {
			e.preventDefault();
			abrirCrearMinisterio();
		});

		btnRefrescar?.addEventListener('click', (e) => {
			e.preventDefault();
			cargarMinisterios();
		});

		btnEditar?.addEventListener('click', (e) => {
			e.preventDefault();
			const id = e.currentTarget.dataset.id;
			abrirEditarMinisterio(id);
		});

		tablaBody?.addEventListener('click', (e) => {
			const btn = e.target.closest('[data-accion]');
			if (!btn) return;
			const id = btn.dataset.id;
			const accion = btn.dataset.accion;
			if (accion === 'editar') abrirEditarMinisterio(id);
			if (accion === 'eliminar') eliminarMinisterio(id);
		});

		const autoCrear =
			flags?.dataset.autoCrear === 'true' || storage.getItem('ministerios_auto_crear') === '1';
		const editarId = flags?.dataset.editarId || storage.getItem('ministerios_editar_id');

		if (autoCrear) {
			storage.removeItem('ministerios_auto_crear');
			abrirCrearMinisterio();
		} else if (editarId) {
			storage.removeItem('ministerios_editar_id');
			abrirEditarMinisterio(editarId);
		}
	});

	// Exponer helpers
	window.cargarMinisterios = cargarMinisterios;
	window.crearMinisterio = abrirCrearMinisterio;
	window.editarMinisterio = abrirEditarMinisterio;
	window.abrirCrearMinisterio = abrirCrearMinisterio;
	window.abrirEditarMinisterio = abrirEditarMinisterio;
	window.eliminarMinisterio = eliminarMinisterio;
})();
