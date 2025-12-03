$(document).ready(function () {
	const token = localStorage.getItem('adminToken');
	let uri = window.location.href.split ("/");
	const idEvento = uri.at (uri.length -1);
	let admin = {};
	try {
		admin = JSON.parse(localStorage.getItem('adminData') || '{}');
	} catch (e) {}

	if (admin && admin.idMinisterio) {
		$('#CampoMinisterio').val(admin.idMinisterio);
	}

	$('#formulario').on('submit', async function (e) {
		e.preventDefault();
		const form = this;
		const formData = new FormData(form);

		const fileInput = $('#CampoFoto')[0];
		if (fileInput && fileInput.files && fileInput.files.length > 0) {
			formData.set('Foto', fileInput.files[0]);
		}

		const headers = token ? { Authorization: `Bearer ${token}` } : {};

		try {
			const res = await fetch(`/Api/Eventos/Editar/${idEvento}`, {
				method: 'PUT',
				body: formData,
				headers, // no Content-Type when FormData
			});

			if (!res.ok) {
				const txt = await res.text();
				throw new Error(txt || 'No se pudo crear el evento.');
			}

			Swal.fire('¡Éxito!', 'El evento fue editado.', 'success').then(() => {
				window.location.href = '/Admin/Dashboard';
			});
		} catch (err) {
			Swal.fire('Error', err.message || 'Fallo al editar el evento.', 'error');
		}
	});
});
