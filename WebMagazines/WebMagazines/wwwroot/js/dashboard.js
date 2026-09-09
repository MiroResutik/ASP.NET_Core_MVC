


$.getJSON('/Dashboard/GetChartData', function (data) {

    // Get revenue chart data and render the chart
    new Chart(document.getElementById('revenueChart'), {
        type: 'line',
        data: {
            labels: data.monthlyRevenue.map(r => r.label),
            datasets: [{
                label: 'Revenue',
                data: data.monthlyRevenue.map(r => r.revenue),
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    // Get orders chart data and render the chart
    new Chart(document.getElementById('ordersChart'), {
        type: 'bar',
        data: {
            labels: data.monthlyOrders.map(r => r.label),
            datasets: [{
                label: 'Orders by Month',
                data: data.monthlyOrders.map(r => r.count),
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    // Get categories chart data and render the chart
    new Chart(document.getElementById('categoryChart'), {
        type: 'bar',
        data: {
            labels: data.productsPerCategory.map(r => r.category),
            datasets: [{
                label: 'Product Categories',
                data: data.productsPerCategory.map(r => r.count),
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    // Get status chart data and render the chart
    new Chart(document.getElementById('statusChart'), {
        type: 'pie',
        data: {
            labels: data.statusBreakdown.map(r => r.status),
            datasets: [{
                label: 'Order Status',
                data: data.statusBreakdown.map(r => r.count),
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
})


