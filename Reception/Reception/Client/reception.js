const API_BASE = 'https://localhost:7066/api/v1';

function showMessage(text, isError = false) {
    const messageDiv = document.getElementById('message');
    messageDiv.textContent = text;
    messageDiv.className = isError ? 'error' : 'success';
    setTimeout(() => messageDiv.textContent = '', 3000);
}

async function addPatient() {
    const name = document.getElementById('name').value;
    const surname = document.getElementById('surname').value;
    const dateOfBirth = document.getElementById('birthdate').value;

    const patientData = {
        name: name,
        surname: surname,
        dateOfBirth: dateOfBirth
    };

    const response = await fetch(`${API_BASE}/patients`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(patientData)
    });

    if (response.ok) {
        await loadPatients();
    } else {
        showMessage('Ошибка при добавлении пациента: ' + response.status, true);
    }
    clearForm();
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
            Статус: ${getStatusText(patient.status)}<br>
            ID: ${patient.id}<br>
            <button onclick="patchPatientStatus('${patient.id}')" style="background: green; margin-top: 5px;">Изменить статус</button>
            <button onclick="deletePatient('${patient.id}')" style="background: #dc3545; margin-top: 5px;">Удалить</button>
        </div>
    `).join('');
}

function displayFoundPatients(patients) {
    const container = document.getElementById('found-patient');

    container.innerHTML = patients.map(patient => `
        <div class="patient-item">
            <strong>${patient.surname} ${patient.name}</strong><br>
            Дата рождения: ${new Date(patient.birthDate).toLocaleDateString()}<br>
            Статус: ${getStatusText(patient.status)}<br>
            ID: ${patient.id}
            <button onclick="patchPatientStatus('${patient.id}')" style="background: green; margin-top: 5px;">Изменить статус</button>
            <button onclick="deletePatient('${patient.id}')" style="background: #dc3545; margin-top: 5px;">Удалить</button>
        </div>
    `).join('');
}

async function deletePatient(patientId) {
    const response = await fetch(`${API_BASE}/patients/${patientId}`, {
        method: 'DELETE'
    });

    if (response.ok) {
        await loadPatients();
        await searchPatients();
    } else {
        showMessage('Ошибка удаления', true);
    }
}

async function patchPatientStatus(patientId) {
    const response = await fetch(`${API_BASE}/patients/${patientId}`, {
        method: 'PATCH',
    });

    if (response.ok) {
        await loadPatients();
        await searchPatients();
    } else {
        showMessage('Ошибка', true);
    }
}

async function searchPatients() {
    const name = document.getElementById('search-name').value;
    const surname = document.getElementById('search-surname').value;
    
    const response = await fetch(`${API_BASE}/patients/${name}/${surname}`, {
        method: 'GET'
    });

    if (response.ok) {
        const patients = await response.json();
        displayFoundPatients(patients);
    } else {
        showMessage('Ошибка загрузки пациентов: ' + response.status, true);
    }
}

function clearForm() {
    document.getElementById('name').value = '';
    document.getElementById('surname').value = '';
    document.getElementById('birthdate').value = '';
}

document.addEventListener('DOMContentLoaded', function () {
    loadPatients();
});

function getStatusText(status) {
    const statusMap = {
        0: 'Пациент не готов к приему у врача',
        1: 'Пациент готов к приему у врача',
        2: 'Пациент на приеме у врача',
        3: 'Пациент уже был у врача на приеме'
    };
    return statusMap[status] || `Статус: ${status}`;
}