// Highlight the active sidebar link based on current path
document.addEventListener("DOMContentLoaded", function () {
    var links = document.querySelectorAll(".app-nav .nav-link");
    var path = window.location.pathname.toLowerCase();

    links.forEach(function (link) {
        var href = link.getAttribute("href");
        if (href && path.indexOf(href.toLowerCase()) === 0 && href !== "/") {
            link.classList.add("active");
        }
    });

    // Confirm before delete actions
    document.querySelectorAll("form[data-confirm]").forEach(function (form) {
        form.addEventListener("submit", function (e) {
            var message = form.getAttribute("data-confirm") || "Are you sure?";
            if (!confirm(message)) {
                e.preventDefault();
            }
        });
    });

    // Client-side max file size check (20 MB) for a nicer UX before hitting the server
    var MAX_SIZE = 20 * 1024 * 1024;
    document.querySelectorAll("input[type=file][data-max-size-check]").forEach(function (input) {
        input.addEventListener("change", function () {
            var feedback = document.getElementById(input.id + "-feedback");
            if (input.files.length > 0 && input.files[0].size > MAX_SIZE) {
                if (feedback) {
                    feedback.textContent = "File exceeds the 20 MB limit. Please choose a smaller file.";
                    feedback.classList.remove("d-none");
                }
                input.value = "";
            } else if (feedback) {
                feedback.classList.add("d-none");
            }
        });
    });
});
