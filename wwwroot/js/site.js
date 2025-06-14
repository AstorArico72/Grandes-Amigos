document.addEventListener('DOMContentLoaded', function () {
	const secciones = document.querySelectorAll('.seccion-animada');

	const observer = new IntersectionObserver(
		(entries) => {
			entries.forEach((entry) => {
				if (entry.isIntersecting) {
					entry.target.classList.add('seccion-visible');
				}
			});
		},
		{
			threshold: 0.2,
		}
	);

	secciones.forEach((seccion) => {
		observer.observe(seccion);
	});
});
