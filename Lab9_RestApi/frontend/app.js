const API_BASE_URL = 'https://localhost:7147/api';

const processingOrders = new Set();

// Format date to readable string
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

// Create order card element
function createOrderCard(order) {
    const card = document.createElement('div');
    card.className = 'order-card';

    card.innerHTML = `
        <div class="order-header">
            <div>
                <div class="order-id">Order #${order.orderId}</div>
                <div class="order-date">Placed on ${formatDate(order.orderPlaced)}</div>
            </div>
            <div class="order-total">${formatCurrency(order.orderTotal)}</div>
        </div>
        
        <div class="order-info">
            <div class="info-group">
                <span class="info-label">Customer Name</span>
                <span class="info-value">${order.firstName} ${order.lastName}</span>
            </div>
            <div class="info-group">
                <span class="info-label">Email</span>
                <span class="info-value">${order.email}</span>
            </div>
            <div class="info-group">
                <span class="info-label">Phone</span>
                <span class="info-value">${order.phoneNumber}</span>
            </div>
            <div class="info-group">
                <span class="info-label">Address</span>
                <span class="info-value">
                    ${order.addressLine1}${order.addressLine2 ? ', ' + order.addressLine2 : ''}<br>
                    ${order.zipCode} ${order.city}${order.state ? ', ' + order.state : ''}<br>
                    ${order.country}
                </span>
            </div>
        </div>

        <div class="order-status">
            ${
                order.orderProcessed
                    ? '<span class="status-badge processed">✅ Processed</span>'
                    : '<button class="process-btn">Mark as processed</button>'
            }
        </div>
    `;

    if (!order.orderProcessed) {
        const processBtn = card.querySelector('.process-btn');
        processBtn.addEventListener('click', () => markOrderProcessed(order.orderId, processBtn));
    }

    return card;
}

// Fetch and display orders
async function loadOrders(isInitialLoad = false) {
    const urlToGet = `${API_BASE_URL}/orders`;

    const loadingEl = document.getElementById('loading');
    const errorEl = document.getElementById('error');
    const ordersContainer = document.getElementById('orders-container');
    const bottomLoadingEl = document.getElementById('bottom-loading');

    try {
        if (isInitialLoad) {
            loadingEl.style.display = 'block';
            ordersContainer.innerHTML = '';
        } else {
            bottomLoadingEl.style.display = 'flex';
        }
        errorEl.style.display = 'none';

        const response = await axios.get(urlToGet);
        let orders = response.data;

        if (isInitialLoad) {
            loadingEl.style.display = 'none';
        } else {
            bottomLoadingEl.style.display = 'none';
        }

        if (!orders || orders.length === 0) {
            ordersContainer.innerHTML = `
                <div class="empty-state">
                    <h2>No orders found</h2>
                    <p>There are no orders to display at this time.</p>
                </div>
            `;
            return;
        }

        // Sort orders by OrderPlaced date (newest first)
        orders.sort((a, b) => {
            const dateA = new Date(a.orderPlaced);
            const dateB = new Date(b.orderPlaced);
            return dateB - dateA; // Descending order (newest first)
        });

        // Clear container before rendering
        ordersContainer.innerHTML = '';

        orders.forEach(order => {
            const card = createOrderCard(order);
            ordersContainer.appendChild(card);
        });

    } catch (error) {
        if (isInitialLoad) {
            loadingEl.style.display = 'none';
        } else {
            bottomLoadingEl.style.display = 'none';
        }
        errorEl.style.display = 'block';

        if (error.response) {
            errorEl.textContent = `Error ${error.response.status}: ${error.response.statusText} Please make sure the API is running at ${urlToGet}`;
        } else if (error.request) {
            errorEl.textContent = `Unable to connect to the API at ${urlToGet}. Please make sure the server is running.`;
        } else {
            errorEl.textContent = `Error: ${error.message}`;
        }

        console.error('Error loading orders:', error);
    }
}

async function markOrderProcessed(orderId, buttonEl) {
    const urlToPatch = `${API_BASE_URL}/orders/${orderId}/process`;

    // Prevent doouble clicks:
    if (processingOrders.has(orderId)) {
        return;
    }

    processingOrders.add(orderId);
    const originalText = buttonEl.textContent;
    buttonEl.textContent = 'Processing...';
    buttonEl.disabled = true;

    try {
        await axios.patch(urlToPatch);
        await loadOrders(false);
    } catch (error) {
        alert('Failed to mark order as processed. Please try again.');
        console.error(`Error processing order ${orderId}:`, error);
        buttonEl.textContent = originalText;
        buttonEl.disabled = false;
        processingOrders.delete(orderId);
        return;
    }

    processingOrders.delete(orderId);
}



// Load orders when page loads 
document.addEventListener('DOMContentLoaded', () => {
    // Initial load
    loadOrders(true);
    document.getElementById("refreshButton").addEventListener('click', () => {
        loadOrders(true);
    });
});