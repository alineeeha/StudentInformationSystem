const defaultOptions = {
    responsive: true,
    plugins: {
        legend: {
            position: 'top',
            labels: {
                color: '#2c3e50',
                font: {
                    family: 'Segoe UI',
                    size: 12,
                    weight: 'bold'
                }
            }
        },
        title: {
            display: true,
            text: '',
            color: '#2c3e50',
            font: {
                size: 16,
                weight: '600'
            },
            padding: { top: 10, bottom: 30 }
        }
    },
    layout: {
        padding: {
            top: 10,
            bottom: 10,
            left: 10,
            right: 10
        }
    },
    scales: {
        y: {
            beginAtZero: true,
            ticks: { color: '#444' },
            grid: { color: '#e0e0e0' }
        },
        x: {
            ticks: { color: '#444' },
            grid: { color: '#f0f0f0' }
        }
    }
};


function renderStudentsPerCourseChart(labels, data) {
    const ctx = document.getElementById("studentsPerCourseChart").getContext("2d");
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                // kein label!
                data: data,
                backgroundColor: '#007D52',
                borderRadius: 6
            }]
        },
        options: {
            ...defaultOptions,
            plugins: {
                ...defaultOptions.plugins,
                legend: {
                    display: false  // 👈 Legende deaktivieren
                },
                title: {
                    ...defaultOptions.plugins.title
                }
            }
        }
    });
}



function renderCourseFormatPieChart(labels, data) {
    const ctx = document.getElementById("courseFormatChart").getContext("2d");
    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: ['#007D52', '#009B77', '#66BB6A', '#A5D6A7'],
                borderWidth: 1
            }]
        },
        options: {
            ...defaultOptions,
            title: {
                ...defaultOptions.plugins.title,
                text: 'Course Format Distribution'
            },
            scales: {} // no axes for pie
        }
    });
}

function renderEnrollmentsOverTimeChart(labels, data) {
    const ctx = document.getElementById("enrollmentsOverTimeChart").getContext("2d");
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Enrollments per Month',
                data: data,
                fill: false,
                borderColor: '#007D52',
                tension: 0.3,
                pointRadius: 4,
                pointBackgroundColor: '#007D52'
            }]
        },
        options: {
            ...defaultOptions,
            title: {
                ...defaultOptions.plugins.title,
                text: 'Monthly Enrollments'
            }
        }
    });
}

