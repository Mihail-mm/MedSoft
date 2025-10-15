const API_BASE = 'http://localhost:5000/api/v1';

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
            ID: ${patient.id}
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
    } else {
        showMessage('Ошибка удаления', true);
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