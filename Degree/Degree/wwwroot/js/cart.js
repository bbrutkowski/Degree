$(document).ready(function () {
    $(document).on('click', '.add-to-cart', async function (event) {
        event.preventDefault();

        const originalText = this.textContent;
        this.textContent = 'Added';
        this.disabled = true;

        const productId = $(this).data('product-id');
        const title = $(this).data('product-title');
        const price = $(this).data('product-price');
        const category = $(this).data('product-category');

        const product = {
            productId: productId,
            title: title,
            price: parseFloat(price),
            category: category,
        };

        try {
            const response = await fetch('/ShoppingCart/Add', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(product)
            });

            if (response.ok) {
                const cartItemCount = await response.json();
                updateCartIcon(cartItemCount);
            } else {
                console.error('Failed to add product to cart. Code:', response.status);
            }
        } catch (error) {
            console.error('Error while adding to cart:', error);
        }

        setTimeout(() => {
            this.textContent = originalText;
            this.disabled = false;
        }, 1000);

        function updateCartIcon(count) {
            const cartCountElement = document.querySelector('.cart-item-count');
            if (cartCountElement) cartCountElement.textContent = count;
        }
    });
});
