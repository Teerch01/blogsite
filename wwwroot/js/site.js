// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', function () {
    const likeForm = document.getElementById('likeForm');
    const likeButton = document.getElementById('likeButton');
    const likeCount = document.getElementById('likeCount');
    const filledHeart = likeButton.querySelector('.bi-heart-fill');
    const emptyHeart = likeButton.querySelector('.bi-heart');

    likeButton.addEventListener('click', function () {
        const postId = likeForm.dataset.itemid;
        fetch(`/Post/LikePost/${postId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    const currentCount = parseInt(likeCount.textContent);
                    if (filledHeart.style.display === 'none') {
                        filledHeart.style.display = 'inline-block';
                        emptyHeart.style.display = 'none';
                        likeCount.textContent = currentCount + 1;
                    } else {
                        filledHeart.style.display = 'none';
                        emptyHeart.style.display = 'inline-block';
                        likeCount.textContent = currentCount - 1;
                    }
                }
            })
            .catch(error => window.alert('Please login or sign up to like the post.'));
    });
});