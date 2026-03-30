document.addEventListener('DOMContentLoaded', () => {
    const navbar = document.getElementById('navbar');
    window.addEventListener('scroll', () => {
        if (window.scrollY > 50) {
            navbar.classList.add('nav-scrolled');
        } else {
            navbar.classList.remove('nav-scrolled');
        }
    });

    const particleContainer = document.querySelector('.particles');
    const particleCount = 50;

    for (let i = 0; i < particleCount; i++) {
        const particle = document.createElement('div');
        particle.style.position = 'absolute';
        particle.style.width = Math.random() * 2 + 'px';
        particle.style.height = particle.style.width;
        particle.style.background = 'white';
        particle.style.opacity = Math.random() * 0.5;
        particle.style.top = Math.random() * 100 + '%';
        particle.style.left = Math.random() * 100 + '%';
        particle.style.pointerEvents = 'none';
        const duration = 10 + Math.random() * 20;
        particle.style.transition = `all ${duration}s linear`;
        particleContainer.appendChild(particle);
        setTimeout(() => {
            particle.style.transform = `translate(${Math.random() * 200 - 100}px, ${Math.random() * 200 - 100}px)`;
        }, 100);
    }

    const heroBg = document.querySelector('.animate-slow-zoom');
    document.addEventListener('mousemove', (e) => {
        const x = (e.clientX / window.innerWidth - 0.5) * 20;
        const y = (e.clientY / window.innerHeight - 0.5) * 20;
        if (heroBg) {
            heroBg.style.transform = `scale(1.1) translate(${x}px, ${y}px)`;
        }
    });
});
