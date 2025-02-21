document.addEventListener("DOMContentLoaded", function () {
    const buttons = document.querySelectorAll(".quantity button");

    buttons.forEach(button => {
        button.addEventListener("click", function () {
            let countSpan = this.parentElement.querySelector(".count");
            let currentCount = parseInt(countSpan.textContent);

            if (this.classList.contains("plus")) {
                countSpan.textContent = currentCount + 1;
            } else if (this.classList.contains("minus") && currentCount > 0) {
                countSpan.textContent = currentCount - 1;
            }
        });
    });
});
