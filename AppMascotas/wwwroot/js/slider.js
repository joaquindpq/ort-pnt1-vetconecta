// ============================================
// SLIDER DE SERVICIOS
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const serviciosContainer = document.querySelector('.servicios-container');
    const servicioCards = document.querySelectorAll('.servicio-card');

    if (!serviciosContainer || servicioCards.length === 0) return;

    let currentIndex = 0;
    const totalCards = servicioCards.length;
    let cardsToShow = getCardsToShow();

    // Determinar cuántas tarjetas mostrar según el ancho de pantalla
    function getCardsToShow() {
        if (window.innerWidth >= 992) return 3;
        if (window.innerWidth >= 768) return 2;
        return 1;
    }

    // Calcular el ancho de desplazamiento
    function getScrollAmount() {
        const cardWidth = servicioCards[0].offsetWidth;
        const gap = 32; // 2rem = 32px
        return cardWidth + gap;
    }

    // Mover el slider
    function moveSlider(direction) {
        const maxIndex = Math.max(0, totalCards - cardsToShow);

        if (direction === 'next') {
            currentIndex = Math.min(currentIndex + 1, maxIndex);
        } else {
            currentIndex = Math.max(currentIndex - 1, 0);
        }

        const scrollAmount = getScrollAmount();
        const translateX = -(currentIndex * scrollAmount);
        serviciosContainer.style.transform = `translateX(${translateX}px)`;

        updateButtons();
    }

    // Actualizar estado de los botones
    function updateButtons() {
        const maxIndex = Math.max(0, totalCards - cardsToShow);

        if (currentIndex === 0) {
            prevBtn.style.opacity = '0.5';
            prevBtn.style.cursor = 'not-allowed';
        } else {
            prevBtn.style.opacity = '1';
            prevBtn.style.cursor = 'pointer';
        }

        if (currentIndex >= maxIndex) {
            nextBtn.style.opacity = '0.5';
            nextBtn.style.cursor = 'not-allowed';
        } else {
            nextBtn.style.opacity = '1';
            nextBtn.style.cursor = 'pointer';
        }
    }

    // Event listeners
    if (prevBtn) {
        prevBtn.addEventListener('click', () => moveSlider('prev'));
    }

    if (nextBtn) {
        nextBtn.addEventListener('click', () => moveSlider('next'));
    }

    // Reiniciar al cambiar el tamaño de la ventana
    window.addEventListener('resize', () => {
        cardsToShow = getCardsToShow();
        currentIndex = 0;
        serviciosContainer.style.transform = 'translateX(0)';
        updateButtons();
    });

    // Inicializar
    updateButtons();

    // ============================================
    // AUTO SLIDE (Opcional)
    // ============================================
    let autoSlideInterval;

    function startAutoSlide() {
        autoSlideInterval = setInterval(() => {
            const maxIndex = Math.max(0, totalCards - cardsToShow);
            if (currentIndex >= maxIndex) {
                currentIndex = 0;
            } else {
                moveSlider('next');
            }
        }, 5000); // Cambiar cada 5 segundos
    }

    function stopAutoSlide() {
        clearInterval(autoSlideInterval);
    }

    // Iniciar auto-slide
    startAutoSlide();

    // Pausar al pasar el mouse
    serviciosContainer.addEventListener('mouseenter', stopAutoSlide);
    serviciosContainer.addEventListener('mouseleave', startAutoSlide);

    // Soporte para touch/swipe en móviles
    let touchStartX = 0;
    let touchEndX = 0;

    serviciosContainer.addEventListener('touchstart', (e) => {
        touchStartX = e.changedTouches[0].screenX;
    });

    serviciosContainer.addEventListener('touchend', (e) => {
        touchEndX = e.changedTouches[0].screenX;
        handleSwipe();
    });

    function handleSwipe() {
        const swipeThreshold = 50;
        const diff = touchStartX - touchEndX;

        if (Math.abs(diff) > swipeThreshold) {
            if (diff > 0) {
                // Swipe izquierda - siguiente
                moveSlider('next');
            } else {
                // Swipe derecha - anterior
                moveSlider('prev');
            }
        }
    }
});

// ============================================
// SLIDER DE RESEÑAS DE GOOGLE
// ============================================
document.addEventListener('DOMContentLoaded', function () {
    const reviewsPrevBtn = document.getElementById('reviewsPrevBtn');
    const reviewsNextBtn = document.getElementById('reviewsNextBtn');
    const reviewsContainer = document.querySelector('.google-reviews-container');
    const reviewCards = document.querySelectorAll('.google-review-card');

    if (!reviewsContainer || reviewCards.length === 0) return;

    let reviewsCurrentIndex = 0;
    const reviewsTotalCards = reviewCards.length;
    let reviewsCardsToShow = getReviewsCardsToShow();

    // Determinar cuántas tarjetas mostrar según el ancho de pantalla
    function getReviewsCardsToShow() {
        if (window.innerWidth >= 992) return 3;
        if (window.innerWidth >= 768) return 2;
        return 1;
    }

    // Calcular el ancho de desplazamiento
    function getReviewsScrollAmount() {
        const cardWidth = reviewCards[0].offsetWidth;
        const gap = 32; // 2rem = 32px
        return cardWidth + gap;
    }

    // Mover el slider de reseñas
    function moveReviewsSlider(direction) {
        const maxIndex = Math.max(0, reviewsTotalCards - reviewsCardsToShow);

        if (direction === 'next') {
            reviewsCurrentIndex = Math.min(reviewsCurrentIndex + 1, maxIndex);
        } else {
            reviewsCurrentIndex = Math.max(reviewsCurrentIndex - 1, 0);
        }

        const scrollAmount = getReviewsScrollAmount();
        const translateX = -(reviewsCurrentIndex * scrollAmount);
        reviewsContainer.style.transform = `translateX(${translateX}px)`;

        updateReviewsButtons();
    }

    // Actualizar estado de los botones de reseñas
    function updateReviewsButtons() {
        const maxIndex = Math.max(0, reviewsTotalCards - reviewsCardsToShow);

        if (reviewsCurrentIndex === 0) {
            reviewsPrevBtn.style.opacity = '0.5';
            reviewsPrevBtn.style.cursor = 'not-allowed';
        } else {
            reviewsPrevBtn.style.opacity = '1';
            reviewsPrevBtn.style.cursor = 'pointer';
        }

        if (reviewsCurrentIndex >= maxIndex) {
            reviewsNextBtn.style.opacity = '0.5';
            reviewsNextBtn.style.cursor = 'not-allowed';
        } else {
            reviewsNextBtn.style.opacity = '1';
            reviewsNextBtn.style.cursor = 'pointer';
        }
    }

    // Event listeners para reseñas
    if (reviewsPrevBtn) {
        reviewsPrevBtn.addEventListener('click', () => moveReviewsSlider('prev'));
    }

    if (reviewsNextBtn) {
        reviewsNextBtn.addEventListener('click', () => moveReviewsSlider('next'));
    }

    // Reiniciar al cambiar el tamaño de la ventana
    window.addEventListener('resize', () => {
        reviewsCardsToShow = getReviewsCardsToShow();
        reviewsCurrentIndex = 0;
        reviewsContainer.style.transform = 'translateX(0)';
        updateReviewsButtons();
    });

    // Inicializar botones de reseñas
    updateReviewsButtons();

    // ============================================
    // AUTO SLIDE PARA RESEÑAS (Opcional)
    // ============================================
    let reviewsAutoSlideInterval;

    function startReviewsAutoSlide() {
        reviewsAutoSlideInterval = setInterval(() => {
            const maxIndex = Math.max(0, reviewsTotalCards - reviewsCardsToShow);
            if (reviewsCurrentIndex >= maxIndex) {
                reviewsCurrentIndex = 0;
            } else {
                moveReviewsSlider('next');
            }
        }, 6000); // Cambiar cada 6 segundos
    }

    function stopReviewsAutoSlide() {
        clearInterval(reviewsAutoSlideInterval);
    }

    // Iniciar auto-slide para reseñas
    startReviewsAutoSlide();

    // Pausar al pasar el mouse
    reviewsContainer.addEventListener('mouseenter', stopReviewsAutoSlide);
    reviewsContainer.addEventListener('mouseleave', startReviewsAutoSlide);

    // Soporte para touch/swipe en móviles para reseñas
    let reviewsTouchStartX = 0;
    let reviewsTouchEndX = 0;

    reviewsContainer.addEventListener('touchstart', (e) => {
        reviewsTouchStartX = e.changedTouches[0].screenX;
    });

    reviewsContainer.addEventListener('touchend', (e) => {
        reviewsTouchEndX = e.changedTouches[0].screenX;
        handleReviewsSwipe();
    });

    function handleReviewsSwipe() {
        const swipeThreshold = 50;
        const diff = reviewsTouchStartX - reviewsTouchEndX;

        if (Math.abs(diff) > swipeThreshold) {
            if (diff > 0) {
                // Swipe izquierda - siguiente
                moveReviewsSlider('next');
            } else {
                // Swipe derecha - anterior
                moveReviewsSlider('prev');
            }
        }
    }
});
