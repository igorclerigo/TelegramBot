document.addEventListener('DOMContentLoaded', () => {
    // Botão Learn More
    const learnMoreBtn = document.getElementById('learnMoreBtn');
    learnMoreBtn.addEventListener('click', () => {
        alert('Learn more about our features!');
    });

    // Formulário de Contato
    const contactForm = document.getElementById('contactForm');
    contactForm.addEventListener('submit', (event) => {
        event.preventDefault();
        alert('Thank you for contacting us!');
        contactForm.reset();
    });
});
