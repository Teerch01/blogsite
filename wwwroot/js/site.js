// Like functionality for posts
document.addEventListener('DOMContentLoaded', () => {
    const likeForm = document.getElementById('likeForm');
    const likeButton = document.getElementById('likeButton');
    const likeCount = document.getElementById('likeCount');

    if (!likeButton || !likeCount) return;

    const filledHeart = likeButton.querySelector('.bi-heart-fill');
    const emptyHeart = likeButton.querySelector('.bi-heart');

    likeButton.addEventListener('click', handleLike);

    async function handleLike() {
        const postId = likeForm.dataset.itemid;

        try {
            const response = await fetch(`/Post/LikePost/${postId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            const data = await response.json();

            if (data.success) {
                toggleLikeState();
                updateLikeCount();
            }
        } catch (error) {
            window.alert('Please login or sign up to like the post.');
        }
    }

    function toggleLikeState() {
        [filledHeart, emptyHeart].forEach(icon => {
            icon.style.display = icon.style.display === 'none' ? 'inline-block' : 'none';
        });
    }

    function updateLikeCount() {
        const currentCount = parseInt(likeCount.textContent) || 0;
        const isLiked = filledHeart.style.display !== 'none';
        likeCount.textContent = isLiked ? currentCount + 1 : currentCount - 1;
    }
});


// Profile Dropdown with click-away-to-close behavior
document.addEventListener('DOMContentLoaded', () => {
    const profileTrigger = document.getElementById('profile-trigger');
    const sideNav = document.getElementById('side-nav');

    if (!profileTrigger || !sideNav) return;

    profileTrigger.addEventListener('click', (event) => {
        // Toggle the dropdown on trigger click
        sideNav.style.display = (sideNav.style.display === 'block') ? 'none' : 'block';
    });

    document.addEventListener('click', (event) => {
        // Close the dropdown if the click is outside
        if (!sideNav.contains(event.target) && !profileTrigger.contains(event.target)) {
            sideNav.style.display = 'none';
        }
    });
});


// Rich text editor
const editor1Element = document.getElementById('div_editor1');

if (editor1Element) {
    var editor1 = new RichTextEditor(editor1Element); // Pass the element directly

    document.querySelector('form').addEventListener('submit', function() {
        document.getElementById('input').value = editor1.getHTMLCode();
    });
}

    