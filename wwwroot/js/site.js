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
			0: {
				items: 1,
			},
			768: {
				items: 3,
			},
		},
	});
});
