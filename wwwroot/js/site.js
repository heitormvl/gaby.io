// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// iOS-style swipe-to-delete for .ios-list-row elements.
(function () {
    function initSwipeToDelete(container) {
        var rows = container.querySelectorAll('.ios-list-row[data-swipeable]');
        var actionWidth = 84;

        rows.forEach(function (row) {
            var content = row.querySelector('.ios-list-row-content');
            var startX = 0;
            var deltaX = 0;
            var dragging = false;

            row.addEventListener('touchstart', function (e) {
                startX = e.touches[0].clientX;
                dragging = true;
                closeOtherRows(container, row);
            }, { passive: true });

            row.addEventListener('touchmove', function (e) {
                if (!dragging) return;
                deltaX = e.touches[0].clientX - startX;
                if (deltaX < 0) {
                    var clamped = Math.max(deltaX, -actionWidth - 20);
                    content.style.transform = 'translateX(' + clamped + 'px)';
                }
            }, { passive: true });

            row.addEventListener('touchend', function () {
                dragging = false;
                if (deltaX < -actionWidth / 2) {
                    content.style.transform = 'translateX(-' + actionWidth + 'px)';
                    row.classList.add('swiped-open');
                } else {
                    content.style.transform = '';
                    row.classList.remove('swiped-open');
                }
                deltaX = 0;
            });
        });
    }

    function closeOtherRows(container, exceptRow) {
        container.querySelectorAll('.ios-list-row.swiped-open').forEach(function (row) {
            if (row !== exceptRow) {
                row.querySelector('.ios-list-row-content').style.transform = '';
                row.classList.remove('swiped-open');
            }
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.ios-list[data-swipe-container]').forEach(initSwipeToDelete);
    });
})();
