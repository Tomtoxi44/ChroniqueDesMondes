// 🎲 Effets JavaScript pour D&D - Sons et animations

class DndEffects {
    constructor() {
        this.audioContext = null;
        this.sounds = new Map();
        this.initializeAudio();
    }

    async initializeAudio() {
        try {
            this.audioContext = new (window.AudioContext || window.webkitAudioContext)();
            await this.loadSounds();
        } catch (error) {
            console.log('Audio non disponible:', error);
        }
    }

    async loadSounds() {
        const soundFiles = {
            diceRoll: '/sounds/dice-roll.mp3',
            swordClash: '/sounds/sword-clash.mp3',
            spellCast: '/sounds/spell-cast.mp3',
            criticalHit: '/sounds/critical-hit.mp3',
            damage: '/sounds/damage.mp3',
            healing: '/sounds/healing.mp3'
        };

        for (const [name, url] of Object.entries(soundFiles)) {
            try {
                const response = await fetch(url);
                if (response.ok) {
                    const arrayBuffer = await response.arrayBuffer();
                    const audioBuffer = await this.audioContext.decodeAudioData(arrayBuffer);
                    this.sounds.set(name, audioBuffer);
                }
            } catch (error) {
                console.log(`Son ${name} non trouvé:`, error);
            }
        }
    }

    playSound(soundName, volume = 0.5) {
        if (!this.audioContext || !this.sounds.has(soundName)) {
            return;
        }

        try {
            const source = this.audioContext.createBufferSource();
            const gainNode = this.audioContext.createGain();
            
            source.buffer = this.sounds.get(soundName);
            source.connect(gainNode);
            gainNode.connect(this.audioContext.destination);
            gainNode.gain.value = volume;
            
            source.start();
        } catch (error) {
            console.log('Erreur lecture son:', error);
        }
    }

    // 🎲 Animation de lancement de dés
    animateDiceRoll(element, result, diceType = 'd20') {
        if (!element) return;

        element.classList.add('dice-roll-animation');
        
        // Changer le contenu pendant l'animation
        const symbols = {
            'd20': ['🎲', '⚀', '⚁', '⚂', '⚃', '⚄', '⚅'],
            'd6': ['⚀', '⚁', '⚂', '⚃', '⚄', '⚅'],
            'd8': ['🎲', '⚀', '⚁', '⚂', '⚃', '⚄', '⚅', '🎲'],
            'd4': ['▲', '⚀', '⚁', '⚂', '⚃']
        };

        const diceSymbols = symbols[diceType] || symbols['d20'];
        let counter = 0;
        
        const interval = setInterval(() => {
            element.textContent = diceSymbols[Math.floor(Math.random() * diceSymbols.length)];
            counter++;
            
            if (counter > 10) {
                clearInterval(interval);
                element.textContent = result.toString();
                element.classList.remove('dice-roll-animation');
                
                // Son du dé
                this.playSound('diceRoll', 0.3);
                
                // Effet critique
                if (result === 20 && diceType === 'd20') {
                    this.showCriticalEffect(element);
                }
            }
        }, 80);
    }

    // 💥 Effet de coup critique
    showCriticalEffect(element) {
        if (!element) return;

        element.classList.add('critical-hit');
        this.playSound('criticalHit', 0.7);
        
        // Particles dorées
        this.createParticles(element, '#fbbf24', 15);
        
        setTimeout(() => {
            element.classList.remove('critical-hit');
        }, 800);
    }

    // 💫 Système de particules
    createParticles(element, color, count = 10) {
        if (!element) return;

        const rect = element.getBoundingClientRect();
        const container = document.createElement('div');
        container.className = 'magic-particles';
        container.style.position = 'fixed';
        container.style.left = rect.left + 'px';
        container.style.top = rect.top + 'px';
        container.style.width = rect.width + 'px';
        container.style.height = rect.height + 'px';
        container.style.pointerEvents = 'none';
        container.style.zIndex = '1000';
        
        document.body.appendChild(container);

        for (let i = 0; i < count; i++) {
            const particle = document.createElement('div');
            particle.className = 'magic-particle';
            particle.style.background = color;
            particle.style.left = Math.random() * 100 + '%';
            particle.style.animationDelay = Math.random() * 1 + 's';
            particle.style.animationDuration = (2 + Math.random() * 2) + 's';
            
            container.appendChild(particle);
        }

        // Nettoyer après l'animation
        setTimeout(() => {
            if (container.parentNode) {
                container.parentNode.removeChild(container);
            }
        }, 4000);
    }

    // 💢 Animation de dégâts flottants
    showDamageNumber(element, damage, damageType = 'physical') {
        if (!element || !damage) return;

        const rect = element.getBoundingClientRect();
        const damageElement = document.createElement('div');
        damageElement.className = `damage-number ${damageType}`;
        damageElement.textContent = `-${damage}`;
        damageElement.style.position = 'fixed';
        damageElement.style.left = (rect.left + rect.width / 2) + 'px';
        damageElement.style.top = rect.top + 'px';
        damageElement.style.zIndex = '1000';
        damageElement.style.transform = 'translateX(-50%)';
        
        document.body.appendChild(damageElement);
        
        // Son de dégâts
        this.playSound('damage', 0.4);
        
        // Nettoyer après l'animation
        setTimeout(() => {
            if (damageElement.parentNode) {
                damageElement.parentNode.removeChild(damageElement);
            }
        }, 1500);
    }

    // 💚 Animation de soins
    showHealingNumber(element, healing) {
        if (!element || !healing) return;

        const rect = element.getBoundingClientRect();
        const healElement = document.createElement('div');
        healElement.className = 'damage-number healing';
        healElement.textContent = `+${healing}`;
        healElement.style.position = 'fixed';
        healElement.style.left = (rect.left + rect.width / 2) + 'px';
        healElement.style.top = rect.top + 'px';
        healElement.style.zIndex = '1000';
        healElement.style.transform = 'translateX(-50%)';
        
        document.body.appendChild(healElement);
        
        // Son de soin
        this.playSound('healing', 0.4);
        
        // Particules vertes
        this.createParticles(element, '#22c55e', 8);
        
        // Nettoyer après l'animation
        setTimeout(() => {
            if (healElement.parentNode) {
                healElement.parentNode.removeChild(healElement);
            }
        }, 1500);
    }

    // 🔮 Effet de lancement de sort
    showSpellEffect(element, spellSchool = 'evocation') {
        if (!element) return;

        element.classList.add('spell-cast-effect');
        this.playSound('spellCast', 0.6);
        
        // Couleurs par école de magie
        const schoolColors = {
            evocation: '#f97316',    // Orange (feu)
            illusion: '#8b5cf6',     // Violet
            enchantment: '#ec4899',  // Rose
            necromancy: '#374151',   // Gris sombre
            divination: '#06b6d4',   // Cyan
            abjuration: '#3b82f6',   // Bleu
            conjuration: '#22c55e',  // Vert
            transmutation: '#eab308' // Jaune
        };
        
        const color = schoolColors[spellSchool] || '#8b5cf6';
        this.createParticles(element, color, 20);
        
        setTimeout(() => {
            element.classList.remove('spell-cast-effect');
        }, 1000);
    }

    // 📊 Animation de mise à jour de barre de vie
    updateHealthBar(healthBarElement, newPercentage, oldPercentage = 100) {
        if (!healthBarElement) return;

        // Déterminer la classe de santé
        let healthClass = 'full';
        if (newPercentage <= 10) healthClass = 'critical';
        else if (newPercentage <= 25) healthClass = 'low';
        else if (newPercentage <= 50) healthClass = 'medium';
        else if (newPercentage <= 75) healthClass = 'high';
        
        // Mettre à jour les classes
        healthBarElement.className = `health-bar ${healthClass}`;
        
        // Animer la transition
        healthBarElement.style.width = newPercentage + '%';
        
        // Si les PV baissent, effet de dégâts
        if (newPercentage < oldPercentage) {
            const damage = Math.round((oldPercentage - newPercentage) * 0.1); // Estimation
            setTimeout(() => {
                this.showDamageNumber(healthBarElement.parentElement, damage);
            }, 300);
        }
    }
}

// Instance globale
window.dndEffects = new DndEffects();

// Fonctions exposées pour Blazor
window.playDiceRollSound = () => window.dndEffects.playSound('diceRoll', 0.3);
window.animateDiceRoll = (element, result, diceType) => window.dndEffects.animateDiceRoll(element, result, diceType);
window.showCriticalEffect = (element) => window.dndEffects.showCriticalEffect(element);
window.showDamageNumber = (element, damage, type) => window.dndEffects.showDamageNumber(element, damage, type);
window.showHealingNumber = (element, healing) => window.dndEffects.showHealingNumber(element, healing);
window.showSpellEffect = (element, school) => window.dndEffects.showSpellEffect(element, school);
window.updateHealthBar = (element, newPerc, oldPerc) => window.dndEffects.updateHealthBar(element, newPerc, oldPerc);

// Auto-initialisation
document.addEventListener('DOMContentLoaded', () => {
    console.log('🎮 DnD Effects System Ready!');
});