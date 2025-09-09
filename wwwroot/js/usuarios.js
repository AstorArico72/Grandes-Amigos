// wwwroot/js/usuarios.js
// Lista, crea y elimina USUARIOS (públicos, tabla `usuarios`)
// Requiere: SweetAlert2 + Bootstrap JS (ya cargados en el layout del admin).

document.addEventListener('DOMContentLoaded', () => {
	// === Helpers ==============================================================
	const token = localStorage.getItem('adminToken');
	const authHeaders = token ? { Authorization: `Bearer ${token}` } : {};

	const $ = (sel) => document.querySelector(sel);
	const tbody = $('#tabla-usuarios-body');
	const form = $('#form-usuario');
	const modalEl = document.getElementById('usuario-modal');
	const modal = modalEl ? new bootstrap.Modal(modalEl) : null;

	// Devuelve la primera propiedad definida (soporta nombres con y sin acentos)
	const pick = (obj, ...keys) => {
		for (const k of keys) if (obj && obj[k] !== undefined) return obj[k];
		return '';
	};

	// Muestra mensaje de error en tabla
	const showRowError = (msg) => {
		tbody.innerHTML = `<tr><td colspan="4" class="text-center text-danger">${msg}</td></tr>`;
	};

	// === Cargar usuarios ======================================================
	async function cargarUsuarios() {
		tbody.innerHTML =
			'<tr><td colspan="4" class="text-center">Cargando...</td></tr>';

		try {
			const res = await fetch('/Api/Usuarios/Todos', { headers: authHeaders });
			if (res.status === 401) {
				showRowError('Sesión inválida. Volvé a iniciar sesión.');
				return;
			}
			if (!res.ok) throw new Error(`Error ${res.status} al obtener usuarios`);

			const lista = await res.json();
			tbody.innerHTML = '';

			if (!Array.isArray(lista) || lista.length === 0) {
				tbody.innerHTML =
					'<tr><td colspan="4" class="text-center">No hay usuarios registrados.</td></tr>';
				return;
			}

			for (const u of lista) {
				const nombre = pick(u, 'nombre', 'Nombre');
				const tipoDoc = pick(u, 'tipoDocumento', 'TipoDocumento');
				const numDoc = pick(u, 'numDocumento', 'NumDocumento');
				const correo = pick(u, 'correo', 'Correo');

				const tr = document.createElement('tr');
				tr.innerHTML = `
          <td>${nombre}</td>
          <td>${tipoDoc || ''} ${numDoc || ''}</td>
          <td>${correo}</td>
          <td class="text-end">
            <button class="btn btn-sm btn-outline-danger btn-eliminar" data-id="${numDoc}" data-nombre="${nombre}">
              <i class="bi bi-trash"></i>
            </button>
          </td>`;
				tbody.appendChild(tr);
			}
		} catch (err) {
			console.error(err);
			showRowError(err.message || 'No se pudo cargar la lista.');
		}
	}

	// === Crear usuario ========================================================
	if (form) {
		form.addEventListener('submit', async (ev) => {
			ev.preventDefault();
			const data = new FormData(form); // nombres del form = propiedades del modelo

			try {
				const res = await fetch('/Api/Usuarios/Nuevo', {
					method: 'POST',
					headers: authHeaders, // no seteamos Content-Type, lo pone el navegador
					body: data,
				});

				if (!res.ok) {
					const txt = await res.text();
					throw new Error(txt || 'No se pudo crear el usuario.');
				}

				Swal.fire('¡Éxito!', 'El usuario fue creado.', 'success');
				modal && modal.hide();
				form.reset();
				cargarUsuarios();
			} catch (err) {
				Swal.fire(
					'Error',
					err.message || 'Fallo al crear el usuario.',
					'error'
				);
			}
		});

		// Al abrir el modal, limpiamos el form
		modalEl?.addEventListener('show.bs.modal', () => {
			form.reset();
			document.getElementById('NumDocumentoInput')?.removeAttribute('disabled');
			const title = document.getElementById('modal-titulo');
			if (title) title.textContent = 'Nuevo Usuario';
		});
	}

	// === Eliminar usuario =====================================================
	tbody?.addEventListener('click', async (ev) => {
		const btn = ev.target.closest('.btn-eliminar');
		if (!btn) return;

		const id = btn.dataset.id;
		const nombre = btn.dataset.nombre || '';

		const ok = await Swal.fire({
			title: '¿Eliminar usuario?',
			text: `Esta acción no se puede deshacer. ${nombre ? `(${nombre})` : ''}`,
			icon: 'warning',
			showCancelButton: true,
			confirmButtonText: 'Sí, eliminar',
			cancelButtonText: 'Cancelar',
		}).then((r) => r.isConfirmed);

		if (!ok) return;

		try {
			const res = await fetch(
				`/Api/Usuarios/Borrar/${encodeURIComponent(id)}`,
				{
					method: 'DELETE',
					headers: authHeaders,
				}
			);
			if (!res.ok) {
				const txt = await res.text();
				throw new Error(txt || 'No se pudo eliminar.');
			}
			Swal.fire('Eliminado', 'El usuario fue eliminado.', 'success');
			cargarUsuarios();
		} catch (err) {
			Swal.fire('Error', err.message || 'Fallo al eliminar.', 'error');
		}
	});

	// === Botones UI ===========================================================
	document
		.getElementById('btnRefrescarUsuarios')
		?.addEventListener('click', cargarUsuarios);
	// El botón "Nuevo" abre el modal con data-bs-toggle (no necesita JS extra)

	// === Carga inicial ========================================================
	cargarUsuarios();
});
