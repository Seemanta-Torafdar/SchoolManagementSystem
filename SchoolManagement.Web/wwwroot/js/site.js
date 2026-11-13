// ==========================================================================
// E-SCHOOL MOTION & INTERACTIVE SYSTEM
// 1. Lenis Smooth Mouse Scrolling (NSU-style inertial scroll)
// 2. Parallax Image Engine ([data-parallax="img"], .gp-img)
// 3. Scroll-Driven Expanding Card Engine (Notice Section NSU Animation)
// 4. Notice Folder Tab Filtering & Staggered Reveal
// 5. Footer Scroll-Triggered Entrance Motion (Strictly at page bottom)
// ==========================================================================

(function () {
    'use strict';

    let lenis = null;

    // --------------------------------------------------------------------------
    // 1. Lenis Smooth Inertial Mouse Scrolling (Safe Fallback)
    // --------------------------------------------------------------------------
    if (typeof Lenis !== 'undefined') {
        try {
            lenis = new Lenis({
                duration: 1.15,
                easing: (t) => Math.min(1, 1.001 - Math.pow(2, -10 * t)),
                orientation: 'vertical',
                gestureOrientation: 'vertical',
                smoothWheel: true,
                wheelMultiplier: 1.0,
                touchMultiplier: 1.5,
                infinite: false
            });

            function raf(time) {
                lenis.raf(time);
                requestAnimationFrame(raf);
            }
            requestAnimationFrame(raf);
        } catch (e) {
            console.warn('Lenis scroll initialization skipped:', e);
        }
    }

    // --------------------------------------------------------------------------
    // 2. Parallax Image Engine for [data-parallax="img"] & .gp-img
    // Dynamically updates translate & scale on scroll matching NSU
    // --------------------------------------------------------------------------
    function updateParallaxImages() {
        const windowH = window.innerHeight;
        const parallaxImages = document.querySelectorAll('[data-parallax="img"], .gp-img');

        parallaxImages.forEach(img => {
            const container = img.closest('.es-parallax-box') || img.parentElement;
            if (!container) return;
            const rect = container.getBoundingClientRect();

            if (rect.top <= windowH && rect.bottom >= 0) {
                const progress = (windowH - rect.top) / (windowH + rect.height);
                const centered = progress - 0.5;

                const yTranslate = centered * 55;
                const scaleVal = 1.14 - Math.abs(centered) * 0.05;

                img.style.translate = `0px ${yTranslate.toFixed(2)}px`;
                img.style.scale = scaleVal.toFixed(3);
            }
        });
    }

    // --------------------------------------------------------------------------
    // 3. Scroll-Driven Expanding Card Engine (NSU Motion Style)
    // Tied directly to mouse scrolling: expands card, morphs border-radius & shadow
    // --------------------------------------------------------------------------
    function initNoticeScrollExpansion() {
        const stage = document.getElementById('noticeExpandStage');
        const card = document.getElementById('noticeExpandCard');
        if (!stage || !card) return;

        const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
        if (prefersReduced) {
            card.style.transform = 'none';
            card.style.borderRadius = '22px';
            return;
        }

        let isTicking = false;

        function updateExpansion() {
            const rect = stage.getBoundingClientRect();
            const windowH = window.innerHeight;

            // Start expanding when stage enters the lower viewport (92% height)
            // Reach 100% full scale when top of stage reaches upper viewport (18% height)
            const startY = windowH * 0.92;
            const endY = windowH * 0.18;

            let p = (startY - rect.top) / (startY - endY);
            p = Math.max(0, Math.min(1, p));

            const isMobile = window.innerWidth < 768;
            const minScale = isMobile ? 0.93 : 0.88;
            const startRadius = isMobile ? 26 : 36;
            const endRadius = isMobile ? 16 : 22;

            const currentScale = (minScale + (1.0 - minScale) * p).toFixed(4);
            const currentRadius = Math.round(startRadius - (startRadius - endRadius) * p);

            // Dynamic soft elevation shadow
            const shadowY = Math.round(10 + 18 * (1 - p));
            const shadowBlur = Math.round(24 + 28 * (1 - p));
            const shadowAlpha = (0.05 + 0.10 * (1 - p)).toFixed(3);

            card.style.transform = `scale(${currentScale})`;
            card.style.borderRadius = `${currentRadius}px`;
            card.style.boxShadow = `0 ${shadowY}px ${shadowBlur}px rgba(16, 185, 129, ${shadowAlpha}), 0 4px 16px rgba(0, 0, 0, 0.04)`;

            // Parallax banner image sync
            const bannerImg = card.querySelector('[data-parallax="img"], .gp-img');
            if (bannerImg) {
                const zoom = (1.14 - 0.06 * p).toFixed(3);
                const translateY = ((1 - p) * 28).toFixed(2);
                bannerImg.style.scale = zoom;
                bannerImg.style.translate = `0px ${translateY}px`;
            }

            // Stagger reveal of notice circulars once card starts entering (p >= 0.35)
            if (p >= 0.35) {
                const unrevealedRows = card.querySelectorAll('.es-notice-row-smart:not(.is-revealed)');
                unrevealedRows.forEach((row, idx) => {
                    setTimeout(() => {
                        row.classList.add('is-revealed');
                    }, idx * 75);
                });
            }

            isTicking = false;
        }

        function onScroll() {
            if (!isTicking) {
                requestAnimationFrame(updateExpansion);
                isTicking = true;
            }
        }

        if (lenis) {
            lenis.on('scroll', onScroll);
        }
        window.addEventListener('scroll', onScroll, { passive: true });
        window.addEventListener('resize', onScroll);
        updateExpansion();
    }

    // --------------------------------------------------------------------------
    // 4. Scroll-Driven Expanding Card Engine for Teacher & Management Section
    // (NSU Motion Style: card scales up smoothly as user rolls mouse wheel)
    // --------------------------------------------------------------------------
    function initTeachersScrollExpansion() {
        const stage = document.getElementById('teachersExpandStage');
        const card = document.getElementById('teachersExpandCard');
        if (!stage || !card) return;

        const prefersReduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
        if (prefersReduced) {
            card.style.transform = 'none';
            card.style.borderRadius = '22px';
            return;
        }

        let isTicking = false;

        function updateExpansion() {
            const rect = stage.getBoundingClientRect();
            const windowH = window.innerHeight;

            // Start expanding when stage enters lower viewport (92% height)
            // Reach 100% full scale when top of stage reaches upper viewport (18% height)
            const startY = windowH * 0.92;
            const endY = windowH * 0.18;

            let p = (startY - rect.top) / (startY - endY);
            p = Math.max(0, Math.min(1, p));

            const isMobile = window.innerWidth < 768;
            const minScale = isMobile ? 0.93 : 0.88;
            const startRadius = isMobile ? 26 : 36;
            const endRadius = isMobile ? 16 : 22;

            const currentScale = (minScale + (1.0 - minScale) * p).toFixed(4);
            const currentRadius = Math.round(startRadius - (startRadius - endRadius) * p);

            // Dynamic soft elevation shadow
            const shadowY = Math.round(10 + 18 * (1 - p));
            const shadowBlur = Math.round(24 + 28 * (1 - p));
            const shadowAlpha = (0.05 + 0.08 * (1 - p)).toFixed(3);

            card.style.transform = `scale(${currentScale})`;
            card.style.borderRadius = `${currentRadius}px`;
            card.style.boxShadow = `0 ${shadowY}px ${shadowBlur}px rgba(139, 92, 246, ${shadowAlpha}), 0 4px 16px rgba(0, 0, 0, 0.04)`;

            // Subtle parallax float and zoom on background decorative elements
            const decor = card.querySelector('#teachersBgDecor');
            if (decor) {
                const zoom = (1.0 + 0.18 * p).toFixed(3);
                const offsetY = ((1 - p) * 32).toFixed(1);
                decor.style.setProperty('--decor-zoom', zoom);
                decor.style.setProperty('--decor-y', `${offsetY}px`);
                decor.style.setProperty('--decor2-y', `-${offsetY}px`);
            }

            // Smooth upward reveal & fade-in for the cards row
            const teamRow = card.querySelector('.es-team-row');
            if (teamRow) {
                teamRow.style.opacity = (0.75 + 0.25 * p).toFixed(2);
                teamRow.style.transform = `translateY(${((1 - p) * 12).toFixed(1)}px)`;
            }

            isTicking = false;
        }

        function onScroll() {
            if (!isTicking) {
                requestAnimationFrame(updateExpansion);
                isTicking = true;
            }
        }

        if (lenis) {
            lenis.on('scroll', onScroll);
        }
        window.addEventListener('scroll', onScroll, { passive: true });
        window.addEventListener('resize', onScroll);
        updateExpansion();
    }

    // --------------------------------------------------------------------------
    // 5. Notice Section Category Tabs & Folder Staggered Filtering
    // --------------------------------------------------------------------------
    function initNoticeFolderMotion() {
        const noticeSection = document.getElementById('notice-section');
        const filterButtons = document.querySelectorAll('.es-notice-filter-btn');
        const noticeRows = document.querySelectorAll('.es-notice-row-smart');

        if (!noticeRows.length) return;

        function revealRows(rows) {
            rows.forEach((row, idx) => {
                setTimeout(() => {
                    row.classList.add('is-revealed');
                }, idx * 80);
            });
        }

        // Trigger reveal when user scrolls near Notice section
        if ('IntersectionObserver' in window && noticeSection) {
            const observer = new IntersectionObserver((entries, obs) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        revealRows(noticeRows);
                        obs.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.15 });

            observer.observe(noticeSection);
        } else {
            revealRows(noticeRows);
        }

        // Fallback timer so circulars are never blank
        setTimeout(() => {
            revealRows(noticeRows);
        }, 1200);

        // Interactive folder tabs filtering with slide motion
        filterButtons.forEach(btn => {
            btn.addEventListener('click', function (e) {
                e.preventDefault();
                const filter = this.getAttribute('data-filter');

                filterButtons.forEach(b => b.classList.remove('active'));
                this.classList.add('active');

                let matchedRows = [];
                noticeRows.forEach(row => {
                    const rowCat = row.getAttribute('data-category') || 'all';
                    const isMatch = filter === 'all' || rowCat === filter;

                    row.classList.remove('is-revealed');
                    if (isMatch) {
                        row.style.display = 'flex';
                        matchedRows.push(row);
                    } else {
                        row.style.display = 'none';
                    }
                });

                // Staggered slide in for matched items
                setTimeout(() => {
                    revealRows(matchedRows);
                }, 40);
            });
        });
    }

    // --------------------------------------------------------------------------
    // 5. Footer Scroll-Triggered Entrance Motion
    // Triggers ONLY when user scrolls down to the end of the home page!
    // --------------------------------------------------------------------------
    function initFooterMotion() {
        const footer = document.querySelector('.es-footer');
        const footerCols = document.querySelectorAll('.es-footer-col');
        const footerBottom = document.querySelector('.es-footer-bottom');

        if (!footer || !footerCols.length) return;

        function revealFooter() {
            footerCols.forEach((col, idx) => {
                setTimeout(() => {
                    col.classList.add('is-revealed');
                }, idx * 90);
            });
            if (footerBottom) {
                setTimeout(() => {
                    footerBottom.classList.add('is-revealed');
                }, footerCols.length * 90 + 50);
            }
        }

        if ('IntersectionObserver' in window) {
            const observer = new IntersectionObserver((entries, obs) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        revealFooter();
                        obs.unobserve(entry.target);
                    }
                });
            }, { threshold: 0.1 });

            observer.observe(footer);
        } else {
            revealFooter();
        }

        // Check if footer is already visible on load
        const rect = footer.getBoundingClientRect();
        if (rect.top <= window.innerHeight) {
            revealFooter();
        }
    }

    // --------------------------------------------------------------------------
    // Initialize on DOM Ready
    // --------------------------------------------------------------------------
    document.addEventListener('DOMContentLoaded', function () {
        if (lenis) {
            lenis.on('scroll', updateParallaxImages);
        } else {
            window.addEventListener('scroll', updateParallaxImages, { passive: true });
        }
        window.addEventListener('resize', updateParallaxImages);
        updateParallaxImages();

        initNoticeScrollExpansion();
        initTeachersScrollExpansion();
        initNoticeFolderMotion();
        initFooterMotion();
    });

})();
