document.addEventListener('DOMContentLoaded', () => {
    // 1. Obtención de Elementos del DOM
    const track = document.getElementById('carouselTrack');
    const prevButton = document.querySelector('.prev-button');
    const nextButton = document.querySelector('.next-button');
    const carouselWrapper = document.querySelector('.carousel-wrapper');
    const cards = Array.from(track.getElementsByClassName('card'));

    // 2. CONFIGURACIÓN
    const totalOriginalCards = 9;      // Total de tarjetas únicas
    const cardsToShow = 3;            // Tarjetas visibles
    const cloneCount = cardsToShow;   // Número de clones en cada extremo (3)
    const AUTO_PLAY_INTERVAL = 4000;  // Mueve cada 4 segundos

    let autoplayTimer; // Almacena el ID del temporizador de Autoplay
    let currentIndex = 0; // Índice de la primera tarjeta ORIGINAL visible (0 = Tarjeta 1)

    // --- CÁLCULO DINÁMICO DEL ANCHO ---
    // Calcula el ancho de una tarjeta más sus márgenes
    const getCardWidth = () => {
        if (cards.length === 0) return 0;
        const cardStyle = getComputedStyle(cards[0]);
        const marginX = parseFloat(cardStyle.marginLeft) + parseFloat(cardStyle.marginRight);
        return cards[0].offsetWidth + marginX;
    };

    // --- LÓGICA DE MOVIMIENTO ---

    /**
     * Mueve el track a la posición calculada.
     * @param {number} index - El índice (0-8) al que mover.
     * @param {boolean} smooth - Si usa transición (true) o reseteo instantáneo (false).
     */
    function moveToSlide(index, smooth = true) {
        const cardWidth = getCardWidth();

        // La posición debe compensar el ancho de los clones iniciales
        const newPosition = (index + cloneCount) * cardWidth;

        if (!smooth) {
            track.style.transition = 'none';
        } else {
            track.style.transition = 'transform 0.5s ease-in-out';
        }

        track.style.transform = `translateX(-${newPosition}px)`;

        // Forzar un repaint para que el cambio de 'none' a 'smooth' funcione inmediatamente
        if (!smooth) {
            void track.offsetWidth;
        }
    }

    /**
     * Mueve el carrusel una posición hacia la derecha.
     */
    function nextSlide() {
        // Previene avanzar si ya estamos en el clon más lejano.
        if (currentIndex >= totalOriginalCards + 1) return;
        currentIndex++;
        moveToSlide(currentIndex);
    }

    // --- FUNCIONES DE AUTOPLAY ---

    function startAutoplay() {
        if (autoplayTimer) return;
        autoplayTimer = setInterval(nextSlide, AUTO_PLAY_INTERVAL);
    }

    function stopAutoplay() {
        clearInterval(autoplayTimer);
        autoplayTimer = null;
    }

    // --- INICIALIZACIÓN Y EVENT LISTENERS ---

    // 1. Inicializa la posición (oculta los clones iniciales) y empieza el Autoplay
    moveToSlide(currentIndex, false);
    startAutoplay();

    // 2. Control del Autoplay con Hover (pausa si el ratón está encima)
    carouselWrapper.addEventListener('mouseenter', stopAutoplay);
    carouselWrapper.addEventListener('mouseleave', startAutoplay);

    // 3. Lógica del Bucle Infinito: Reseteo instantáneo al final de la transición
    track.addEventListener('transitionend', () => {
        // Si se movió al clon final (ej. T9 -> Clon T1)
        if (currentIndex >= totalOriginalCards) {
            currentIndex = 0; // Se resetea al índice de la Tarjeta 1 original
            moveToSlide(currentIndex, false); // Reseteo SIN transición
        }
        // Si se movió al clon inicial (ej. T1 -> Clon T9)
        else if (currentIndex < 0) {
            currentIndex = totalOriginalCards - 1; // Se resetea al índice de la Tarjeta 9 original
            moveToSlide(currentIndex, false); // Reseteo SIN transición
        }
    });

    // 4. Lógica de Navegación Manual

    nextButton.addEventListener('click', () => {
        stopAutoplay(); // Detiene para evitar doble movimiento
        nextSlide();
        startAutoplay(); // Reinicia el temporizador
    });

    prevButton.addEventListener('click', () => {
        stopAutoplay(); // Detiene para evitar doble movimiento
        // Previene retroceder si ya estamos en el clon más lejano
        if (currentIndex <= -cloneCount) return;
        currentIndex--;
        moveToSlide(currentIndex);
        startAutoplay(); // Reinicia el temporizador
    });

    // 5. Manejar el redimensionamiento de la ventana
    window.addEventListener('resize', () => {
        // Mueve a la posición actual sin transición después de redimensionar
        moveToSlide(currentIndex, false);
    });
});
