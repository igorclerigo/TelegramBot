// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
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