document.addEventListener('DOMContentLoaded', function () {
    const likeForm = document.getElementById('likeForm');
    const likeButton = document.getElementById('likeButton');
    const likeCount = document.getElementById('likeCount');

    if (likeButton) { // Ensure likeButton exists before proceeding
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
    }
});

var editor1 = new RichTextEditor("#div_editor1");
    // Add an event listener to update the hidden input before form submission
    document.querySelector('form').addEventListener('submit', function() {
        document.getElementById('input').value = editor1.getHTMLCode(); 
    });