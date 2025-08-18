// Theme management
window.setTheme = (themeName) => {
    document.documentElement.setAttribute('data-theme', themeName);
    localStorage.setItem('cdm-theme', themeName);
};

// Initialize theme on page load
window.initializeTheme = () => {
    const savedTheme = localStorage.getItem('cdm-theme') || 'dark-fantasy';
    window.setTheme(savedTheme);
};

// Login page animations
window.initializeLoginAnimations = () => {
    const loginCard = document.querySelector('.login-card');
    if (loginCard) {
        loginCard.classList.add('animate-fade-in');
        
        // Add glow effect to form inputs on focus
        const inputs = document.querySelectorAll('.form-control');
        inputs.forEach(input => {
            input.addEventListener('focus', function() {
                this.parentElement.classList.add('focused');
            });
            
            input.addEventListener('blur', function() {
                this.parentElement.classList.remove('focused');
            });
        });
    }
};

// Register page animations
window.initializeRegisterAnimations = () => {
    const registerCard = document.querySelector('.register-card');
    if (registerCard) {
        registerCard.classList.add('animate-fade-in');
        
        // Password strength indicator
        const passwordInput = document.querySelector('#password');
        if (passwordInput) {
            passwordInput.addEventListener('input', function() {
                const strength = calculatePasswordStrength(this.value);
                updatePasswordStrengthIndicator(strength);
            });
        }
    }
};

// Dice animation
window.animateDiceResult = () => {
    const resultTotal = document.querySelector('.result-total');
    if (resultTotal) {
        resultTotal.style.transform = 'scale(0)';
        resultTotal.style.opacity = '0';
        
        setTimeout(() => {
            resultTotal.style.transition = 'all 0.5s cubic-bezier(0.68, -0.55, 0.265, 1.55)';
            resultTotal.style.transform = 'scale(1)';
            resultTotal.style.opacity = '1';
            
            // Add shake effect
            setTimeout(() => {
                resultTotal.style.animation = 'shake 0.5s ease-in-out';
            }, 200);
        }, 100);
    }
};

// Shake animation for dice
const shakeKeyframes = `
@keyframes shake {
    0%, 100% { transform: translateX(0); }
    10%, 30%, 50%, 70%, 90% { transform: translateX(-5px); }
    20%, 40%, 60%, 80% { transform: translateX(5px); }
}`;

// Inject shake animation
if (!document.querySelector('#shake-animation')) {
    const style = document.createElement('style');
    style.id = 'shake-animation';
    style.textContent = shakeKeyframes;
    document.head.appendChild(style);
}

// Password strength calculation
function calculatePasswordStrength(password) {
    let strength = 0;
    
    if (password.length >= 8) strength += 1;
    if (password.match(/[a-z]+/)) strength += 1;
    if (password.match(/[A-Z]+/)) strength += 1;
    if (password.match(/[0-9]+/)) strength += 1;
    if (password.match(/[^a-zA-Z0-9]+/)) strength += 1;
    
    return strength;
}

function updatePasswordStrengthIndicator(strength) {
    const indicator = document.querySelector('.password-strength');
    if (!indicator) {
        // Create indicator if it doesn't exist
        const passwordGroup = document.querySelector('#password').parentElement;
        const strengthDiv = document.createElement('div');
        strengthDiv.className = 'password-strength';
        strengthDiv.innerHTML = `
            <div class="strength-bar">
                <div class="strength-fill"></div>
            </div>
            <span class="strength-text">Faible</span>
        `;
        passwordGroup.appendChild(strengthDiv);
    }
    
    const fill = document.querySelector('.strength-fill');
    const text = document.querySelector('.strength-text');
    
    if (fill && text) {
        const percentage = (strength / 5) * 100;
        fill.style.width = percentage + '%';
        
        let strengthText = 'Très faible';
        let strengthClass = 'very-weak';
        
        if (strength >= 4) {
            strengthText = 'Très fort';
            strengthClass = 'very-strong';
        } else if (strength >= 3) {
            strengthText = 'Fort';
            strengthClass = 'strong';
        } else if (strength >= 2) {
            strengthText = 'Moyen';
            strengthClass = 'medium';
        } else if (strength >= 1) {
            strengthText = 'Faible';
            strengthClass = 'weak';
        }
        
        text.textContent = strengthText;
        fill.className = `strength-fill ${strengthClass}`;
    }
}

// Smooth scrolling for navigation
window.smoothScrollTo = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
    }
};

// Parallax effect for backgrounds
window.initializeParallax = () => {
    const parallaxElements = document.querySelectorAll('.parallax');
    
    if (parallaxElements.length > 0) {
        window.addEventListener('scroll', () => {
            const scrolled = window.pageYOffset;
            const rate = scrolled * -0.5;
            
            parallaxElements.forEach(element => {
                element.style.transform = `translateY(${rate}px)`;
            });
        });
    }
};

// Particle animation for background
window.initializeParticles = () => {
    const canvas = document.getElementById('particles-canvas');
    if (!canvas) return;
    
    const ctx = canvas.getContext('2d');
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    
    const particles = [];
    const particleCount = 50;
    
    class Particle {
        constructor() {
            this.x = Math.random() * canvas.width;
            this.y = Math.random() * canvas.height;
            this.vx = (Math.random() - 0.5) * 0.5;
            this.vy = (Math.random() - 0.5) * 0.5;
            this.size = Math.random() * 2 + 1;
            this.opacity = Math.random() * 0.5 + 0.2;
        }
        
        update() {
            this.x += this.vx;
            this.y += this.vy;
            
            if (this.x < 0 || this.x > canvas.width) this.vx *= -1;
            if (this.y < 0 || this.y > canvas.height) this.vy *= -1;
        }
        
        draw() {
            ctx.save();
            ctx.globalAlpha = this.opacity;
            ctx.fillStyle = '#8B4513';
            ctx.beginPath();
            ctx.arc(this.x, this.y, this.size, 0, Math.PI * 2);
            ctx.fill();
            ctx.restore();
        }
    }
    
    for (let i = 0; i < particleCount; i++) {
        particles.push(new Particle());
    }
    
    function animate() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        
        particles.forEach(particle => {
            particle.update();
            particle.draw();
        });
        
        requestAnimationFrame(animate);
    }
    
    animate();
    
    window.addEventListener('resize', () => {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    });
};

// Initialize everything when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    window.initializeTheme();
    window.initializeParallax();
    window.initializeParticles();
});