$(document).ready (function () {
    const token = localStorage.getItem ("adminToken");
    let admin = localStorage.getItem ("adminData");
    const authHeaders = token ? { Authorization: `Bearer ${token}` } : {};
    $("#CampoMinisterio").attr ("value", admin.idMinisterio);

    let form = $("#formulario");
    form.on ("submit", async (e) => {
        e.preventDefault ();
        const formData = new FormData(form);
        try {
			const res = await fetch('/Api/Eventos/Nuevo', {
				method: 'POST',
				body: formData,
                headers: authHeaders
			});
            if (!res.ok) {
				const txt = await res.text();
				throw new Error(txt || 'No se pudo crear el usuario.');
			} else {
			    Swal.fire('¡Éxito!', 'El evento fue creado.', 'success');
            }
        } catch (err) {
				Swal.fire(
				'Error',
				err.message || 'Fallo al crear el evento.',
				'error'
			);
		}
    });
});