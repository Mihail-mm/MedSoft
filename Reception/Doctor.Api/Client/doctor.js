const API_BASE = 'https://localhost:7174/api/v1/patients';

function showMessage(text, isError = false) {
    const messageDiv = document.getElementById('message');
    messageDiv.textContent = text;
    messageDiv.className = isError ? 'error' : 'success';
    setTimeout(() => messageDiv.textContent = '', 3000);
}

async function loadPatients() {
    const response = await fetch(`${API_BASE}`);

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
            Статус: ${getStatusText(patient.status)}<br>
            ID: ${patient.id}
           <button onclick="startAppointment('${patient.id}')" style="background: green; margin-top: 5px;">Начать приём</button>
            <button onclick="finishAppointment('${patient.id}')" style="background: #dc3545; margin-top: 5px;">Закончить прием</button>
        </div>
    `).join('');
}

async function startAppointment(patientId) {
    const response = await fetch(`${API_BASE}/${patientId}/start`, {
        method: 'PATCH',
    });
}

async function finishAppointment(patientId) {
    const response = await fetch(`${API_BASE}/${patientId}/finish`, {
        method: 'PATCH',
    });
    
    if (!response.ok) {
        showMessage("Ошибка: нельзя закончить приём", true);
    }
}

function startAutoRefresh() {
    loadPatients();
    refreshTimer = setInterval(loadPatients, 3000);
}


document.addEventListener('DOMContentLoaded', function () {
    startAutoRefresh();
});

function getStatusText(status) {
    const statusMap = {
        0: 'Пациент зарегистрирован в медицинской системе',
        1: 'Пациент готов к приему у врача',
        2: 'Пациент на приеме у врача',
        3: 'Пациент уже был у врача на приеме'
    };
    return statusMap[status] || `Статус: ${status}`;
}