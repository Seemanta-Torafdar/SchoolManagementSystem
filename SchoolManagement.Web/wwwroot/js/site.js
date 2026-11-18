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
            card.style.boxShadow = `0 ${shadowY}px ${shadowBlur}px rgba(28, 43, 33, ${shadowAlpha}), 0 4px 16px rgba(28, 43, 33, 0.05)`;

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
            card.style.boxShadow = `0 ${shadowY}px ${shadowBlur}px rgba(28, 43, 33, ${shadowAlpha}), 0 4px 16px rgba(28, 43, 33, 0.05)`;

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
    // 5. Guaranteed Footer Injection & Entrance Motion
    // --------------------------------------------------------------------------
    function ensureFooterAndMotion() {
        let footer = document.querySelector('.es-footer') || document.querySelector('footer');
        if (!footer) {
            footer = document.createElement('footer');
            footer.className = 'es-footer';
            footer.innerHTML = `
                <div class="container">
                    <div class="row g-4 pb-4">
                        <!-- Brand Info -->
                        <div class="col-lg-4 col-md-6 es-footer-col is-revealed">
                            <div class="d-flex align-items-center gap-2 mb-3">
                                <div class="es-brand-icon" style="width:38px;height:38px;border-radius:10px;">
                                    <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round">
                                        <path d="M4 19.5v-15A2.5 2.5 0 0 1 6.5 2H20v20H6.5a2.5 2.5 0 0 1-2.5-2.5Z"></path>
                                        <path d="M6 2v20"></path>
                                    </svg>
                                </div>
                                <span class="fs-4 fw-bold text-white">E-School</span>
                            </div>
                            <p class="small mb-4 text-white-50" style="line-height: 1.7; max-width: 320px;">
                                A modern, smart online school platform to learn, explore, and stay connected with your classes, teachers, events, and results.
                            </p>
                            <div class="d-flex align-items-center gap-2">
                                <a href="#" class="es-social-btn" aria-label="Facebook"><i data-lucide="facebook" style="width:16px;height:16px;"></i></a>
                                <a href="#" class="es-social-btn" aria-label="Instagram"><i data-lucide="instagram" style="width:16px;height:16px;"></i></a>
                                <a href="#" class="es-social-btn" aria-label="Twitter"><i data-lucide="twitter" style="width:16px;height:16px;"></i></a>
                                <a href="#" class="es-social-btn" aria-label="Youtube"><i data-lucide="youtube" style="width:16px;height:16px;"></i></a>
                            </div>
                        </div>

                        <!-- Quick Navigation -->
                        <div class="col-lg-2 col-md-3 col-6 es-footer-col is-revealed">
                            <h6 class="es-footer-heading">Navigation</h6>
                            <ul class="es-footer-links">
                                <li><a href="/Home">Home</a></li>
                                <li><a href="/Notices">Notice</a></li>
                                <li><a href="/Teachers">Teachers</a></li>
                                <li><a href="/About">Management</a></li>
                                <li><a href="/Results">Result</a></li>
                            </ul>
                        </div>

                        <!-- Academics & Resources -->
                        <div class="col-lg-3 col-md-3 col-6 es-footer-col is-revealed">
                            <h6 class="es-footer-heading">Academics</h6>
                            <ul class="es-footer-links">
                                <li><a href="/Academics">Curriculum</a></li>
                                <li><a href="/Routine">Class Routine</a></li>
                                <li><a href="/Events">Upcoming Events</a></li>
                                <li><a href="/Gallery">Campus Gallery</a></li>
                                <li><a href="/Admission">Online Admission</a></li>
                            </ul>
                        </div>

                        <!-- Contact Information -->
                        <div class="col-lg-3 col-md-6 es-footer-col is-revealed">
                            <h6 class="es-footer-heading">Get in Touch</h6>
                            <ul class="es-footer-links small">
                                <li class="d-flex align-items-center gap-2 text-white-50">
                                    <i data-lucide="phone" style="width:16px;height:16px;color:#687F65 !important;"></i>
                                    <span>+880 1712-345678</span>
                                </li>
                                <li class="d-flex align-items-center gap-2 text-white-50">
                                    <i data-lucide="mail" style="width:16px;height:16px;color:#687F65 !important;"></i>
                                    <span>info@eschool.edu.bd</span>
                                </li>
                                <li class="d-flex align-items-center gap-2 text-white-50">
                                    <i data-lucide="map-pin" style="width:16px;height:16px;color:#687F65 !important;"></i>
                                    <span>Green Road, Dhanmondi, Dhaka-1205</span>
                                </li>
                            </ul>
                        </div>
                    </div>

                    <!-- Copyright -->
                    <div class="es-footer-bottom is-revealed">
                        <span>&copy; ${new Date().getFullYear()} E-School. All rights reserved.</span>
                        <div class="d-flex align-items-center gap-3">
                            <a href="#">Privacy Policy</a>
                            <span>&bull;</span>
                            <a href="#">Terms of Use</a>
                        </div>
                    </div>
                </div>
            `;
            const main = document.querySelector('main') || document.body;
            if (main.parentElement && main !== document.body) {
                main.parentElement.appendChild(footer);
            } else {
                document.body.appendChild(footer);
            }
            if (typeof lucide !== 'undefined') {
                lucide.createIcons();
            }
        }

        footer.style.setProperty('display', 'block', 'important');
        footer.style.setProperty('visibility', 'visible', 'important');
        footer.style.setProperty('opacity', '1', 'important');

        const footerCols = footer.querySelectorAll('.es-footer-col');
        const footerBottom = footer.querySelector('.es-footer-bottom');

        footerCols.forEach(col => {
            col.classList.add('is-revealed');
            col.style.setProperty('opacity', '1', 'important');
            col.style.setProperty('transform', 'none', 'important');
        });
        if (footerBottom) {
            footerBottom.classList.add('is-revealed');
            footerBottom.style.setProperty('opacity', '1', 'important');
            footerBottom.style.setProperty('transform', 'none', 'important');
        }
    }

    // --------------------------------------------------------------------------
    // 6. Dynamic Seamless Campus Zone & Unified Card initialization
    // (Enforces solid #EAEFE1 background across all 5 sections, #F1F2E8 cards with #1C2B21 borders)
    // --------------------------------------------------------------------------
    function initUnifiedCampusZone() {
        try {
            // Inject guaranteed seamless styles in document.head to bypass CSS caching
            if (!document.getElementById('es-seamless-campus-styles')) {
                const style = document.createElement('style');
                style.id = 'es-seamless-campus-styles';
                style.textContent = `
                    .es-unified-showcase-zone,
                    .es-unified-academic-zone,
                    #notice-section,
                    #teachers-section,
                    #event-section,
                    #videos-section,
                    #result-section,
                    section:has(.es-notice-card),
                    section:has(.es-teachers-expand-card),
                    section:has(.es-event-info-card),
                    section:has(.es-video-card),
                    section:has(.es-result-card) {
                        background: #EAEFE1 !important;
                        border: none !important;
                        border-top: none !important;
                        border-bottom: none !important;
                        border-left: none !important;
                        border-right: none !important;
                        box-shadow: none !important;
                        outline: none !important;
                        margin-top: 0 !important;
                        margin-bottom: 0 !important;
                    }

                    .es-notice-card,
                    .es-teachers-expand-card,
                    .es-event-info-card,
                    .es-videos-container-card,
                    .es-result-card {
                        background: #F1F2E8 !important;
                        border: 2px solid #1C2B21 !important;
                        border-radius: 28px !important;
                        box-shadow: 0 20px 48px rgba(28, 43, 33, 0.12), 0 6px 18px rgba(28, 43, 33, 0.05) !important;
                    }

                    .es-video-card {
                        background: #F1F2E8 !important;
                        border: 2px solid #1C2B21 !important;
                        border-radius: 20px !important;
                        box-shadow: 0 12px 30px rgba(28, 43, 33, 0.08) !important;
                    }

                    .es-video-body {
                        background: #F1F2E8 !important;
                        color: #1C2B21 !important;
                    }

                    .es-event-photo {
                        border: 2px solid #1C2B21 !important;
                        border-radius: 20px !important;
                        box-shadow: 0 16px 36px rgba(28, 43, 33, 0.12) !important;
                    }

                    /* Transparent Floating Navbar */
                    .es-navbar-wrapper {
                        position: absolute !important;
                        top: 0 !important;
                        left: 0 !important;
                        width: 100% !important;
                        z-index: 1030 !important;
                        background: transparent !important;
                        backdrop-filter: none !important;
                        -webkit-backdrop-filter: none !important;
                        border: none !important;
                        border-bottom: none !important;
                        box-shadow: none !important;
                        transition: background 0.35s ease, box-shadow 0.35s ease, backdrop-filter 0.35s ease !important;
                    }

                    .es-navbar-wrapper.is-scrolled {
                        position: fixed !important;
                        background: rgba(28, 43, 33, 0.94) !important;
                        backdrop-filter: blur(14px) !important;
                        -webkit-backdrop-filter: blur(14px) !important;
                        border-bottom: 1px solid rgba(210, 217, 197, 0.18) !important;
                        box-shadow: 0 8px 25px rgba(28, 43, 33, 0.25) !important;
                    }

                    /* Hero Banner with image directly applied */
                    .es-full-bg-showcase,
                    #quick-actions,
                    .es-hero {
                        min-height: 100vh !important;
                        display: flex !important;
                        flex-direction: column !important;
                        justify-content: space-between !important;
                        padding-top: 108px !important;
                        padding-bottom: 36px !important;
                        position: relative !important;
                        z-index: 10 !important;
                        background: linear-gradient(90deg, 
                            rgba(15, 23, 42, 0.86) 0%, 
                            rgba(15, 23, 42, 0.70) 42%, 
                            rgba(15, 23, 42, 0.24) 75%, 
                            rgba(15, 23, 42, 0.08) 100%
                        ), url('/images/hero-building.jpg') center center / cover no-repeat !important;
                        border: none !important;
                        border-bottom: none !important;
                        box-shadow: none !important;
                    }

                    /* Transparent Quick Action Buttons Strip */
                    .es-quick-actions-wrapper {
                        background: rgba(255, 255, 255, 0.06) !important;
                        backdrop-filter: blur(14px) !important;
                        -webkit-backdrop-filter: blur(14px) !important;
                        border-radius: 20px !important;
                        padding: 10px 14px !important;
                        box-shadow: 0 12px 32px rgba(0, 0, 0, 0.25) !important;
                        border: 1.5px solid rgba(255, 255, 255, 0.18) !important;
                    }

                    .es-quick-btn {
                        background: rgba(255, 255, 255, 0.10) !important;
                        backdrop-filter: blur(8px) !important;
                        -webkit-backdrop-filter: blur(8px) !important;
                        border: 1.5px solid rgba(255, 255, 255, 0.25) !important;
                        color: #FFFFFF !important;
                        text-shadow: 0 1px 3px rgba(0, 0, 0, 0.4) !important;
                    }

                    .es-quick-btn .es-quick-icon {
                        background: rgba(255, 255, 255, 0.16) !important;
                        color: var(--btn-color, #FFFFFF) !important;
                    }

                    .es-quick-btn:hover {
                        background: rgba(255, 255, 255, 0.24) !important;
                        border-color: var(--btn-color, rgba(255, 255, 255, 0.7)) !important;
                        color: #FFFFFF !important;
                        box-shadow: 0 10px 25px var(--btn-glow, rgba(0, 0, 0, 0.35)) !important;
                    }

                    .es-quick-btn:hover .es-quick-icon {
                        background: var(--btn-color, #10B981) !important;
                        color: #FFFFFF !important;
                    }
                `;
                document.head.appendChild(style);
            }

            // Ensure header is styled as transparent floating and handle scroll
            const navbar = document.querySelector('.es-navbar-wrapper');
            if (navbar) {
                const handleNavScroll = function () {
                    if (window.scrollY > 40) {
                        navbar.classList.add('is-scrolled');
                    } else {
                        navbar.classList.remove('is-scrolled');
                    }
                };
                window.addEventListener('scroll', handleNavScroll, { passive: true });
                handleNavScroll();
            }

            // Hero banner styling
            const heroShowcase = document.querySelector('.es-full-bg-showcase') || document.getElementById('quick-actions');
            if (heroShowcase) {
                heroShowcase.style.setProperty('min-height', '100vh', 'important');
                heroShowcase.style.setProperty('padding-top', '108px', 'important');
                heroShowcase.style.setProperty('background', "linear-gradient(90deg, rgba(15, 23, 42, 0.86) 0%, rgba(15, 23, 42, 0.70) 42%, rgba(15, 23, 42, 0.24) 75%, rgba(15, 23, 42, 0.08) 100%), url('/images/hero-building.jpg') center center / cover no-repeat", 'important');
                heroShowcase.style.setProperty('border', 'none', 'important');
                heroShowcase.style.setProperty('border-bottom', 'none', 'important');
                heroShowcase.style.setProperty('box-shadow', 'none', 'important');
            }

            // Quick action buttons transparent glassmorphism
            const quickWrapper = document.querySelector('.es-quick-actions-wrapper');
            if (quickWrapper) {
                quickWrapper.style.setProperty('background', 'rgba(255, 255, 255, 0.06)', 'important');
                quickWrapper.style.setProperty('backdrop-filter', 'blur(14px)', 'important');
                quickWrapper.style.setProperty('-webkit-backdrop-filter', 'blur(14px)', 'important');
                quickWrapper.style.setProperty('border', '1.5px solid rgba(255, 255, 255, 0.18)', 'important');
                quickWrapper.style.setProperty('box-shadow', '0 12px 32px rgba(0, 0, 0, 0.25)', 'important');
            }

            document.querySelectorAll('.es-quick-btn').forEach(function (btn) {
                btn.style.setProperty('background', 'rgba(255, 255, 255, 0.10)', 'important');
                btn.style.setProperty('border', '1.5px solid rgba(255, 255, 255, 0.25)', 'important');
                btn.style.setProperty('color', '#FFFFFF', 'important');
                const icon = btn.querySelector('.es-quick-icon');
                if (icon) {
                    icon.style.setProperty('background', 'rgba(255, 255, 255, 0.16)', 'important');
                }
            });

            // Target all 5 sections
            const noticeSec = document.getElementById('notice-section') || document.querySelector('section:has(.es-notice-card)');
            const teachersSec = document.getElementById('teachers-section') || document.querySelector('section:has(.es-teachers-expand-card)');
            const eventSec = document.getElementById('event-section') || document.querySelector('section:has(.es-event-info-card)');
            const videoSec = document.getElementById('videos-section') || document.querySelector('section:has(.es-video-card)');
            const resultSec = document.getElementById('result-section') || document.querySelector('section:has(.es-result-card)');

            const sections = [noticeSec, teachersSec, eventSec, videoSec, resultSec];
            sections.forEach(function (sec) {
                if (!sec) return;
                sec.style.setProperty('background', '#EAEFE1', 'important');
                sec.style.setProperty('border', 'none', 'important');
                sec.style.setProperty('border-top', 'none', 'important');
                sec.style.setProperty('border-bottom', 'none', 'important');
                sec.style.setProperty('box-shadow', 'none', 'important');
                sec.style.setProperty('outline', 'none', 'important');
                sec.style.setProperty('margin-top', '0', 'important');
                sec.style.setProperty('margin-bottom', '0', 'important');
            });

            // Clean any existing wrapper zones to share #EAEFE1
            document.querySelectorAll('.es-unified-showcase-zone, .es-unified-academic-zone').forEach(function (z) {
                z.style.setProperty('background', '#EAEFE1', 'important');
                z.style.setProperty('border', 'none', 'important');
                z.style.setProperty('box-shadow', 'none', 'important');
                z.style.setProperty('margin-top', '0', 'important');
                z.style.setProperty('margin-bottom', '0', 'important');
            });

            // Videos container card wrapping & styling
            if (videoSec) {
                videoSec.id = 'videos-section';
                const row = videoSec.querySelector('.row.g-4');
                if (row && !row.parentElement.classList.contains('es-videos-container-card')) {
                    const cardWrap = document.createElement('div');
                    cardWrap.className = 'es-videos-container-card';
                    row.parentElement.insertBefore(cardWrap, row);
                    cardWrap.appendChild(row);
                }
            }

            // Force cards styling
            document.querySelectorAll('.es-videos-container-card').forEach(function (c) {
                c.style.setProperty('background', '#F1F2E8', 'important');
                c.style.setProperty('border', '2px solid #1C2B21', 'important');
                c.style.setProperty('border-radius', '28px', 'important');
                c.style.setProperty('padding', '28px', 'important');
            });

            document.querySelectorAll('.es-video-card').forEach(function (vc) {
                vc.style.setProperty('background', '#F1F2E8', 'important');
                vc.style.setProperty('border', '2px solid #1C2B21', 'important');
                vc.style.setProperty('border-radius', '20px', 'important');
                const vb = vc.querySelector('.es-video-body');
                if (vb) {
                    vb.style.setProperty('background', '#F1F2E8', 'important');
                }
            });

            document.querySelectorAll('.es-event-info-card').forEach(function (ec) {
                ec.style.setProperty('background', '#F1F2E8', 'important');
                ec.style.setProperty('border', '2px solid #1C2B21', 'important');
                ec.style.setProperty('border-radius', '28px', 'important');
            });

            document.querySelectorAll('.es-result-card').forEach(function (rc) {
                rc.style.setProperty('background', '#F1F2E8', 'important');
                rc.style.setProperty('border', '2px solid #1C2B21', 'important');
                rc.style.setProperty('border-radius', '28px', 'important');
            });
        } catch (e) {
            console.error('Error in initUnifiedCampusZone:', e);
        }
    }

    // --------------------------------------------------------------------------
    // 6. Modern Sliding Spotlight Navbar Transition Engine
    // Provides pixel-perfect magnetic sliding pill indicator on hover/active
    // Full support for both desktop horizontal capsule and mobile vertical menu
    // --------------------------------------------------------------------------
    function initNavbarTransitions() {
        const navMenu = document.getElementById('esNavMenu');
        const slider = document.getElementById('esNavSlider');
        const navbarCollapse = document.getElementById('esNavbarContent');
        if (!navMenu || !slider) return;

        const navLinks = Array.from(navMenu.querySelectorAll('.es-nav-link'));
        if (navLinks.length === 0) return;

        let activeLink = navLinks.find(link => link.classList.contains('active')) || navLinks[0];

        function updateSlider(targetLink, isHover) {
            if (!targetLink || !slider || !navMenu) {
                if (slider) slider.style.opacity = '0';
                return;
            }

            // Element must be in DOM and visible
            const menuRect = navMenu.getBoundingClientRect();
            const linkRect = targetLink.getBoundingClientRect();

            if (linkRect.width <= 0 || linkRect.height <= 0) {
                slider.style.opacity = '0';
                return;
            }

            const targetLeft = Math.round(linkRect.left - menuRect.left + navMenu.scrollLeft);
            const targetTop = Math.round(linkRect.top - menuRect.top + navMenu.scrollTop);
            const targetWidth = Math.round(linkRect.width);
            const targetHeight = Math.round(linkRect.height);

            slider.style.opacity = '1';
            slider.style.transform = `translate3d(${targetLeft}px, ${targetTop}px, 0)`;
            slider.style.width = `${targetWidth}px`;
            slider.style.height = `${targetHeight}px`;

            if (isHover) {
                slider.style.background = 'rgba(255, 255, 255, 0.28)';
            } else {
                slider.style.background = 'rgba(255, 255, 255, 0.20)';
            }
        }

        function handlePointerMove(e) {
            const target = e.target;
            const link = target ? target.closest('.es-nav-link') : null;
            if (link) {
                updateSlider(link, true);
            }
        }

        // Attach mouse & pointer tracking across the navbar menu (Desktop & Mobile)
        navMenu.addEventListener('mousemove', handlePointerMove);
        navMenu.addEventListener('pointermove', handlePointerMove);
        navMenu.addEventListener('mouseover', handlePointerMove);

        // Individual link listeners for direct focus/hover
        navLinks.forEach(link => {
            link.addEventListener('mouseenter', () => updateSlider(link, true));
            link.addEventListener('focus', () => updateSlider(link, true));
            link.addEventListener('touchstart', () => updateSlider(link, true), { passive: true });

            link.addEventListener('click', () => {
                navLinks.forEach(l => l.classList.remove('active'));
                link.classList.add('active');
                activeLink = link;
                updateSlider(link, false);
            });
        });

        // Restore to active item when mouse/pointer leaves the nav menu
        navMenu.addEventListener('mouseleave', () => {
            updateSlider(activeLink, false);
        });
        navMenu.addEventListener('pointerleave', () => {
            updateSlider(activeLink, false);
        });

        // Bootstrap Collapse Listeners for Mobile Dropdown
        if (navbarCollapse) {
            navbarCollapse.addEventListener('show.bs.collapse', () => {
                setTimeout(() => updateSlider(activeLink, false), 50);
                setTimeout(() => updateSlider(activeLink, false), 150);
                setTimeout(() => updateSlider(activeLink, false), 350);
            });

            navbarCollapse.addEventListener('shown.bs.collapse', () => {
                updateSlider(activeLink, false);
            });

            navbarCollapse.addEventListener('hide.bs.collapse', () => {
                if (window.innerWidth < 992) {
                    slider.style.opacity = '0';
                }
            });

            navbarCollapse.addEventListener('hidden.bs.collapse', () => {
                if (window.innerWidth < 992) {
                    slider.style.opacity = '0';
                }
            });
        }

        // Responsive window resize
        let resizeTimer;
        window.addEventListener('resize', () => {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(() => {
                updateSlider(activeLink, false);
            }, 60);
        }, { passive: true });

        // Initial measurements
        setTimeout(() => {
            updateSlider(activeLink, false);
        }, 100);
        setTimeout(() => {
            updateSlider(activeLink, false);
        }, 300);
    }

    // --------------------------------------------------------------------------
    // 7. Interactive Button Dynamic Ripple Engine
    // Generates a smooth expanding radial ripple on click
    // --------------------------------------------------------------------------
    function initButtonRippleEffect() {
        document.addEventListener('click', function (e) {
            const btn = e.target.closest('.btn-es-login, .btn-es-green, .btn-es-glass, .es-quick-btn, .btn-es-register, .btn-es-result');
            if (!btn) return;

            const rect = btn.getBoundingClientRect();
            const ripple = document.createElement('span');
            ripple.className = 'es-click-ripple';
            const size = Math.max(rect.width, rect.height) * 2;
            ripple.style.width = `${size}px`;
            ripple.style.height = `${size}px`;
            ripple.style.left = `${e.clientX - rect.left - size / 2}px`;
            ripple.style.top = `${e.clientY - rect.top - size / 2}px`;
            btn.appendChild(ripple);

            setTimeout(() => {
                ripple.remove();
            }, 600);
        });
    }

    // Immediate run
    initUnifiedCampusZone();
    ensureFooterAndMotion();
    initNavbarTransitions();
    initButtonRippleEffect();

    // --------------------------------------------------------------------------
    // Initialize on DOM Ready
    // --------------------------------------------------------------------------
    document.addEventListener('DOMContentLoaded', function () {
        initUnifiedCampusZone();
        ensureFooterAndMotion();
        initNavbarTransitions();
        initButtonRippleEffect();

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
    });

})();
