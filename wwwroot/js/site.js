document.addEventListener('DOMContentLoaded', function () {
	// Animaciones de entrada de secciones
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

// OwlCarousel
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

// Validación del registro
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

	// Validar al salir del campo
	Object.entries(inputs).forEach(([campo, input]) => {
		input.addEventListener('blur', () => {
			const mensaje = validarCampo(campo, input.value.trim(), inputs);
			if (mensaje) {
				Swal.fire({
					icon: 'warning',
					title: `Campo ${campo}`,
					text: mensaje,
				}).then(() => input.focus());
			}
		});
	});

	// Validación al enviar
	form.addEventListener('submit', function (e) {
		e.preventDefault();

		let errores = [];
		let primerError = null;

		for (const [campo, input] of Object.entries(inputs)) {
			const valor = input.value.trim();
			const mensaje = validarCampo(campo, valor, inputs);
			if (mensaje) {
				errores.push(`<li>${mensaje}</li>`);
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

		// Si todo está OK, enviar por fetch
		const datos = new FormData(form);
		fetch('/Api/Inscritos/Nuevo', {
			method: 'POST',
			body: datos,
		})
			.then((resp) => {
				if (!resp.ok) throw new Error('Registro fallido');
				return resp.text();
			})
			.then(() => {
				Swal.fire({
					icon: 'success',
					title: '¡Registro exitoso!',
					text: 'Ya podés iniciar sesión con tu documento y clave.',
				});
				form.reset();
				document.querySelector('#login-tab').click();
			})
			.catch(() => {
				Swal.fire({
					icon: 'error',
					title: 'No se pudo registrar',
					text: 'Verificá si el documento ya está registrado.',
				});
			});
	});

	// Validación campo por campo
	function validarCampo(campo, valor, inputs) {
		switch (campo) {
			case 'Nombre':
				if (!/^[a-zA-ZÁÉÍÓÚáéíóúñÑ\s]{3,}$/.test(valor))
					return 'El nombre debe tener al menos 3 letras y solo letras.';
				break;
			case 'Correo':
				if (!valor.includes('@') || valor.length < 5) return 'Correo inválido.';
				break;
			case 'NumDocumento':
				if (!/^\d{7,9}$/.test(valor))
					return 'DNI inválido: debe contener solo números (7 a 9 cifras).';
				break;
			case 'Teléfono':
				if (!/^\+54\s?9\s?\d{2,4}\s?\d{3,4}\s?\d{3,4}$/.test(valor))
					return 'Teléfono inválido. Usá formato +54 9 266 123 456.';
				break;
			case 'Clave':
				if (valor.length < 6)
					return 'La contraseña debe tener al menos 6 caracteres.';
				break;
			case 'RepetirClave':
				const original = inputs.Clave.value.trim();
				if (valor !== original) return 'Las contraseñas no coinciden.';
				break;
		}
		return null;
	}
});

// Mostrar/ocultar contraseña (ojito)
document.addEventListener('click', function (e) {
	const toggle = e.target.closest('.toggle-pass');
	if (toggle) {
		const inputId = toggle.dataset.target;
		const input = document.getElementById(inputId);
		const icon = toggle.querySelector('i');

		if (input.type === 'password') {
			input.type = 'text';
			icon.classList.remove('bi-eye');
			icon.classList.add('bi-eye-slash');
		} else {
			input.type = 'password';
			icon.classList.remove('bi-eye-slash');
			icon.classList.add('bi-eye');
		}
	}
});
