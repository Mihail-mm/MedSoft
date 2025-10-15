const API_BASE = 'http://localhost:5001/api/v1';

function showMessage(text, isError = false) {
    const messageDiv = document.getElementById('message');
    messageDiv.textContent = text;
    messageDiv.className = isError ? 'error' : 'success';
    setTimeout(() => messageDiv.textContent = '', 3000);
}

async function loadPatients() {
    const response = await fetch(`${API_BASE}/patients`);

    if (response.ok) {
        const patients = await response.json();
        displayPatients(patients);
    } else {
        showMessage('Ошибка загрузки пациентов: ' + response.status, true);
    }
}

function displayPatients(patients) {
    const container = document.getElementById('patients');

    container.innerHTML = patients.map(patient => `
        <div class="patient-item">
            <strong>${patient.surname} ${patient.name}</strong><br>
            Дата рождения: ${new Date(patient.birthDate).toLocaleDateString()}<br>
            ID: ${patient.id}
        </div>
    `).join('');
}

function startAutoRefresh() {
    loadPatients();
    refreshTimer = setInterval(loadPatients, 1000);
}
document.addEventListener('DOMContentLoaded', function () {
    startAutoRefresh();
});