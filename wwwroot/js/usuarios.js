document.addEventListener('DOMContentLoaded', function () {
	const token = localStorage.getItem('adminToken');
	const authHeaders = {
		Authorization: `Bearer ${token}`,
	};

	const modalElement = document.getElementById('usuario-modal');
	const usuarioModal = new bootstrap.Modal(modalElement);
	const formUsuario = document.getElementById('form-usuario');
	const tablaUsuariosBody = document.getElementById('tabla-usuarios-body');

	// --- FUNCIÓN PRINCIPAL: Cargar y mostrar todos los usuarios ---
	async function cargarUsuarios() {
		tablaUsuariosBody.innerHTML =
			'<tr><td colspan="4" class="text-center">Cargando...</td></tr>';

		try {
			const response = await fetch('/Api/Usuarios/Todos', {
				headers: authHeaders,
			});
			if (!response.ok) {
				throw new Error(
					`Error ${response.status}: No se pudo obtener la lista de usuarios.`
				);
			}
			const usuarios = await response.json();

			tablaUsuariosBody.innerHTML = ''; // Limpiar la tabla antes de llenar
			if (usuarios.length === 0) {
				tablaUsuariosBody.innerHTML =
					'<tr><td colspan="4" class="text-center">No hay usuarios registrados.</td></tr>';
			} else {
				usuarios.forEach((user) => {
					const fila = `
                        <tr>
                            <td>${user.nombre}</td>
                            <td>${user.tipoDocumento} ${user.numDocumento}</td>
                            <td>${user.correo}</td>
                            <td class="text-end">
                                <button class="btn btn-sm btn-outline-danger btn-eliminar" data-id="${user.numDocumento}" data-nombre="${user.nombre}">
                                    <i class="bi bi-trash"></i>
                                </button>
                            </td>
                        </tr>`;
					tablaUsuariosBody.insertAdjacentHTML('beforeend', fila);
				});
			}
		} catch (error) {
			console.error('Error al cargar usuarios:', error);
			tablaUsuariosBody.innerHTML = `<tr><td colspan="4" class="text-center text-danger">${error.message}</td></tr>`;
		}
	}

	// --- MANEJO DEL FORMULARIO (Crear Usuario) ---
	formUsuario.addEventListener('submit', async function (event) {
		event.preventDefault();

		const formData = new FormData(formUsuario);

		// El endpoint de tu controlador espera los datos de un formulario [FromForm]
		// y se encarga del hashing de la clave en el backend.
		try {
			const response = await fetch('/Api/Usuarios/Nuevo', {
				method: 'POST',
				headers: { ...authHeaders }, // Solo el token, el Content-Type lo pone el navegador
				body: formData,
			});

			if (response.ok) {
				Swal.fire(
					'¡Éxito!',
					'El usuario ha sido creado correctamente.',
					'success'
				);
				usuarioModal.hide(); // Ocultar el modal
				cargarUsuarios(); // Recargar la lista de usuarios
			} else {
				const errorTexto = await response.text();
				throw new Error(errorTexto || 'No se pudo crear el usuario.');
			}
		} catch (error) {
			Swal.fire('Error', error.message, 'error');
		}
	});

	// Limpiar el formulario cuando se abre el modal para un nuevo usuario
	modalElement.addEventListener('show.bs.modal', function () {
		formUsuario.reset();
		document.getElementById('modal-titulo').textContent = 'Nuevo Usuario';
		// Habilitar el campo NumDocumentoInput por si se deshabilitó en un modo de edición futuro
		document.getElementById('NumDocumentoInput').disabled = false;
	});

	// --- MANEJO DE ELIMINACIÓN DE USUARIOS ---
	tablaUsuariosBody.addEventListener('click', function (event) {
		const botonEliminar = event.target.closest('.btn-eliminar');
		if (botonEliminar) {
			const userId = botonEliminar.dataset.id;
			const userName = botonEliminar.dataset.nombre;

			Swal.fire({
				title: `¿Estás seguro?`,
				text: `¡No podrás revertir la eliminación de ${userName}!`,
				icon: 'warning',
				showCancelButton: true,
				confirmButtonColor: '#d33',
				cancelButtonColor: '#3085d6',
				confirmButtonText: 'Sí, ¡eliminar!',
				cancelButtonText: 'Cancelar',
			}).then(async (result) => {
				if (result.isConfirmed) {
					try {
						const response = await fetch(`/Api/Usuarios/Borrar/${userId}`, {
							method: 'DELETE',
							headers: authHeaders,
						});

						if (response.ok) {
							Swal.fire(
								'¡Eliminado!',
								'El usuario ha sido eliminado.',
								'success'
							);
							cargarUsuarios(); // Recargar la lista
						} else {
							const errorTexto = await response.text();
							throw new Error(errorTexto || 'No se pudo eliminar el usuario.');
						}
					} catch (error) {
						Swal.fire('Error', error.message, 'error');
					}
				}
			});
		}
	});

	// --- Carga inicial de datos ---
	cargarUsuarios();
});
