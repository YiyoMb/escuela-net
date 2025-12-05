// ============================================
// CONFIGURACIÓN GENERAL
// ============================================

// URL base de la API (ajusta el puerto según tu configuración)
const API_URL = 'http://localhost:5125/api';

// ============================================
// FUNCIONES DE NAVEGACIÓN
// ============================================

function mostrarSeccion(seccion) {
    // Ocultar todas las secciones
    document.querySelectorAll('.seccion').forEach(sec => {
        sec.classList.add('d-none');
    });
    
    // Remover clase activa de todos los links
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
    });
    
    // Mostrar la sección seleccionada
    switch(seccion) {
        case 'listado':
            document.getElementById('seccionListado').classList.remove('d-none');
            cargarListadoCompleto();
            break;
        case 'escuelas':
            document.getElementById('seccionEscuelas').classList.remove('d-none');
            cargarEscuelas();
            break;
        case 'padres':
            document.getElementById('seccionPadres').classList.remove('d-none');
            cargarPadres();
            break;
        case 'alumnos':
            document.getElementById('seccionAlumnos').classList.remove('d-none');
            cargarAlumnos();
            cargarSelectores(); // Cargar padres, madres y escuelas en los selectores
            break;
    }
    
    // Activar el link correspondiente
    event.target.classList.add('active');
}

// ============================================
// FUNCIONES DE UTILIDAD
// ============================================

function mostrarError(mensaje) {
    alert('❌ Error: ' + mensaje);
}

function mostrarExito(mensaje) {
    alert('✅ ' + mensaje);
}

function calcularEdad(fechaNacimiento) {
    const hoy = new Date();
    const nacimiento = new Date(fechaNacimiento);
    let edad = hoy.getFullYear() - nacimiento.getFullYear();
    const mes = hoy.getMonth() - nacimiento.getMonth();
    
    if (mes < 0 || (mes === 0 && hoy.getDate() < nacimiento.getDate())) {
        edad--;
    }
    
    return edad;
}


// ============================================
// LISTADO COMPLETO DE ALUMNOS
// ============================================

async function cargarListadoCompleto() {
    try {
        const response = await fetch(`${API_URL}/alumnos/completo`);
        
        if (!response.ok) {
            throw new Error('Error al cargar el listado');
        }
        
        const alumnos = await response.json();
        const tbody = document.getElementById('tablaListadoCompleto');
        
        if (alumnos.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center text-muted">
                        <i class="bi bi-inbox"></i> No hay alumnos registrados
                    </td>
                </tr>
            `;
            return;
        }
        
        tbody.innerHTML = alumnos.map(alumno => `
            <tr>
                <td>${alumno.id}</td>
                <td>
                    <strong>${alumno.nombreCompleto}</strong><br>
                    <small class="text-muted">${alumno.fechaNacimiento}</small>
                </td>
                <td>${alumno.edad} años</td>
                <td><span class="badge bg-info">${alumno.grado || 'N/A'}</span></td>
                <td>
                    <strong>${alumno.padre.nombreCompleto}</strong><br>
                    <small class="text-muted">
                        ${alumno.padre.telefono || 'Sin teléfono'}<br>
                        ${alumno.padre.email || 'Sin email'}
                    </small>
                </td>
                <td>
                    <strong>${alumno.madre.nombreCompleto}</strong><br>
                    <small class="text-muted">
                        ${alumno.madre.telefono || 'Sin teléfono'}<br>
                        ${alumno.madre.email || 'Sin email'}
                    </small>
                </td>
                <td>
                    <strong>${alumno.escuela.nombre}</strong><br>
                    <small class="text-muted">
                        ${alumno.escuela.direccion || ''}<br>
                        ${alumno.escuela.telefono || ''}
                    </small>
                </td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error:', error);
        document.getElementById('tablaListadoCompleto').innerHTML = `
            <tr>
                <td colspan="7" class="text-center text-danger">
                    <i class="bi bi-exclamation-triangle"></i> Error al cargar los datos
                </td>
            </tr>
        `;
    }
}


// ============================================
// GESTIÓN DE ESCUELAS
// ============================================

async function cargarEscuelas() {
    try {
        const response = await fetch(`${API_URL}/escuelas`);
        const escuelas = await response.json();
        const tbody = document.getElementById('tablaEscuelas');
        
        if (escuelas.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center text-muted">No hay escuelas registradas</td>
                </tr>
            `;
            return;
        }
        
        tbody.innerHTML = escuelas.map(escuela => `
            <tr>
                <td>${escuela.id}</td>
                <td><strong>${escuela.nombre}</strong></td>
                <td>${escuela.direccion || '-'}</td>
                <td>${escuela.telefono || '-'}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editarEscuela(${escuela.id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="eliminarEscuela(${escuela.id})">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error:', error);
        mostrarError('No se pudieron cargar las escuelas');
    }
}

// ============================================
// FORMULARIO DE ESCUELAS
// ============================================

document.getElementById('formEscuela').addEventListener('submit', async (e) => {
    e.preventDefault();

    const id = document.getElementById('escuelaId').value;
    const escuela = {
        nombre: document.getElementById('escuelaNombre').value,
        direccion: document.getElementById('escuelaDireccion').value,
        telefono: document.getElementById('escuelaTelefono').value
    };

    try {
        let response;

        if(id) {
            //Actualizar
            escuela.id = parseInt(id);
            response = await fetch(`${API_URL}/escuelas/${id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(escuela)
            });
        } else {
            // Crear
            response = await fetch(`${API_URL}/escuelas`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(escuela)
            });
        }

        if(response.ok) {
            mostrarExito(id ? 'Escuela actualizada correctamente' : 'Escuela creada correctamente');
            limpiarFormEscuela();
            cargarEscuelas();
        } else {
            const error = await response.json();
            mostrarError(error.message || 'Error al guardar la escuela');
        }
    } catch (error) {
        console.error('Error:', error);
        mostrarError('Error al guardar la escuela');
    }
});

async function editarEscuela(id) {
    try {
        const response = await fetch(`${API_URL}/escuelas/${id}`);
        const escuela = await response.json();

        document.getElementById('escuelaId').value = escuela.id;
        document.getElementById('escuelaNombre').value = escuela.nombre;
        document.getElementById('escuelaDireccion').value = escuela.direccion || '';
        document.getElementById('escuelaTelefono').value = escuela.telefono || '';

        document.getElementById('tituloFormEscuela').innerHTML = '<i class="bi bi-pencil"></i> Ediitar escuela';

        //Scroll al formulario
        document.getElementById('formEscuela').scrollIntoView({ behavior:'smooth' });
    } catch (error) {
        console.error('Error: ', error);
        mostrarError('Error al cargar la escuela');
    }
}

async function eliminarEscuela(id) {
    if (!confirm('¿Estás seguro de eliminar esta escuela?')) {
        return;
    }

    try {
        const response = await fetch(`${API_URL}/escuelas/${id}`, {
            method: 'DELETE'
        });

        if(response.ok) {
            mostrarExito('Escuela eliminada correctamente');
            cargarEscuelas();
        } else {
            const error = await response.json();
            mostrarError(error.message || 'Error al eliminar la escuela');
        }
    } catch (error) {
        console.error('Error: ', error);
        mostrarError('Error al eliminar la escuela');
    }
}

function limpiarFormEscuela() {
    document.getElementById('formEscuela').reset();
    document.getElementById('escuelaId').value = '';
    document.getElementById('tituloFormEscuela').innerHTML = 
        '<i class="bi bi-plus-circle"></i> Nueva Escuela';
}

// ============================================
// GESTIÓN DE PADRES
// ============================================

async function cargarPadres() {
    try {
        const response = await fetch(`${API_URL}/padres`);
        const padres = await response.json();
        const tbody = document.getElementById('tablaPadres');

        if (padres.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="6" class="text-center text-muted">No hay padres registrados</td>
                </tr>
            `;
            return;
        }

        tbody.innerHTML = padres.map(padre => `
            <tr>
                <td>${padre.id}</td>
                <td>
                    <span class="badge ${padre.esPadre ? 'bg-primary' : 'bg-pink'}">
                        ${padre.esPadre ? 'Padre' : 'Madre'}
                    </span>
                </td>
                <td><strong>${padre.nombreCompleto}</strong></td>
                <td>${padre.telefono || '-'}</td>
                <td>${padre.email || '-'}</td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editarPadre(${padre.id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="eliminarPadre(${padre.id})">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');

    } catch (error) {
        console.error('Error:', error);
        mostrarError('No se pudieron cargar los padres');
    }
}

// ============================================
// FORMULARIO DE PADRES
// ============================================

document.getElementById('formPadre').addEventListener('submit', async (e) => {
    e.preventDefault();

    const id = document.getElementById('padreId').value;
    const padre = {
        nombre: document.getElementById('padreNombre').value,
        apellido: document.getElementById('padreApellido').value,
        telefono: document.getElementById('padreTelefono').value,
        email: document.getElementById('padreEmail').value,
        esPadre: document.getElementById('padreTipo').value === 'true'
    };

    try {
        let response;

        if (id) {
            // Actualizar
            padre.id = parseInt(id);
            response = await fetch(`${API_URL}/padres/${id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(padre)
            });
        } else {
            // Crear
            response = await fetch(`${API_URL}/padres`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(padre)
            });
        }

        if (response.ok) {
            mostrarExito(id ? 'Padre/Madre actualizado correctamente' : 'Padre/Madre creado correctamente');
            limpiarFormPadre();
            cargarPadres();
        } else {
            const error = await response.json();
            mostrarError(error.message || 'Error al guardar');
        }

    } catch (error) {
        console.error('Error: ', error);
        mostrarError('Error al guardar el padre/madre');
    }
});

async function editarPadre(id) {
    try {
        const response = await fetch(`${API_URL}/padres/${id}`);
        const padre = await response.json();
        
        document.getElementById('padreId').value = padre.id;
        document.getElementById('padreTipo').value = padre.esPadre.toString();
        document.getElementById('padreNombre').value = padre.nombre;
        document.getElementById('padreApellido').value = padre.apellido;
        document.getElementById('padreTelefono').value = padre.telefono || '';
        document.getElementById('padreEmail').value = padre.email || '';
        
        document.getElementById('tituloFormPadre').innerHTML = 
            '<i class="bi bi-pencil"></i> Editar Padre/Madre';
        
        document.getElementById('formPadre').scrollIntoView({ behavior: 'smooth' });
        
    } catch (error) {
        console.error('Error:', error);
        mostrarError('Error al cargar el padre/madre');
    }
}

async function eliminarPadre(id) {
    if (!confirm('¿Estás seguro de eliminar este padre/madre?')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_URL}/padres/${id}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            mostrarExito('Padre/Madre eliminado correctamente');
            cargarPadres();
        } else {
            const error = await response.json();
            mostrarError(error.message || 'Error al eliminar');
        }
        
    } catch (error) {
        console.error('Error:', error);
        mostrarError('Error al eliminar el padre/madre');
    }
}

function limpiarFormPadre() {
    document.getElementById('formPadre').reset();
    document.getElementById('padreId').value = '';
    document.getElementById('tituloFormPadre').innerHTML = '<i class="bi bi-plus-circle"></i> Nuevo Padre/Madre';
}

// ============================================
// GESTIÓN DE ALUMNOS
// ============================================

async function cargarAlumnos() {
    try {
        const response = await fetch(`${API_URL}/alumnos`);
        const alumnos = await response.json();
        const tbody = document.getElementById('tablaAlumnos');
        
        if (alumnos.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center text-muted">No hay alumnos registrados</td>
                </tr>
            `;
            return;
        }
        
        tbody.innerHTML = alumnos.map(alumno => `
            <tr>
                <td>${alumno.id}</td>
                <td><strong>${alumno.nombreCompleto}</strong></td>
                <td>${calcularEdad(alumno.fechaNacimiento)} años</td>
                <td><span class="badge bg-info">${alumno.grado || 'N/A'}</span></td>
                <td>
                    <button class="btn btn-sm btn-primary" onclick="editarAlumno(${alumno.id})">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="eliminarAlumno(${alumno.id})">
                        <i class="bi bi-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');
        
    } catch (error) {
        console.error('Error:', error);
        mostrarError('No se pudieron cargar los alumnos');
    }
}

// ============================================
// CARGAR SELECTORES (PADRES, MADRES, ESCUELAS)
// ============================================

async function cargarSelectores() {
    try {
        //Cargar Padres
        const responsePapas = await fetch(`${API_URL}/padres/papas`);
        const papas = await responsePapas.json();
        const selectPadre = document.getElementById('alumnoPadreId');
        selectPadre.innerHTML = '<option value="">Seleccionar padre...</option>' +
            papas.map(p => `<option value="${p.id}">${p.nombreCompleto}</option>`).join('');

        //Cargar Madres
        const responseMamas = await fetch (`${API_URL}/padres/mamas`);
        const mamas = await responseMamas.json();
        const selectMadre = document.getElementById('alumnoMadreId');
        selectMadre.innerHTML = '<option value="">Seleccionar madre...</option>' +
            mamas.map(m => `<option value="${m.id}">${m.nombreCompleto}</option>`).join('');

        //Cargar escuelas
        const responseEscuelas = await fetch(`${API_URL}/escuelas`);
        const escuelas = await responseEscuelas.json();
        const selectEscuela = document.getElementById('alumnoEscuelaId');
        selectEscuela.innerHTML = '<option value="">Seleccionar escuela...</option>' +
            escuelas.map(e => `<option value="${e.id}">${e.nombre}</option>`).join('');

    } catch (error) {
        console.error('Error: ', error);
        mostrarError('Error al cargar los selectores');
    }
}

// ============================================
// FORMULARIO DE ALUMNOS
// ============================================

document.getElementById('formAlumno').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const id = document.getElementById('alumnoId').value;
    
    // Obtener y formatear la fecha correctamente
    const fechaInput = document.getElementById('alumnoFechaNacimiento').value;
    const fechaISO = fechaInput + 'T00:00:00.000Z';
    
    const alumno = {
        nombre: document.getElementById('alumnoNombre').value.trim(),
        apellido: document.getElementById('alumnoApellido').value.trim(),
        fechaNacimiento: fechaISO,
        grado: document.getElementById('alumnoGrado').value.trim() || null,
        padreId: parseInt(document.getElementById('alumnoPadreId').value),
        madreId: parseInt(document.getElementById('alumnoMadreId').value),
        escuelaId: parseInt(document.getElementById('alumnoEscuelaId').value)
    };
    
    // Validaciones básicas en el frontend
    if (!alumno.nombre || !alumno.apellido) {
        mostrarError('El nombre y apellido son obligatorios');
        return;
    }
    
    if (!alumno.padreId || isNaN(alumno.padreId)) {
        mostrarError('Debes seleccionar un padre');
        return;
    }
    
    if (!alumno.madreId || isNaN(alumno.madreId)) {
        mostrarError('Debes seleccionar una madre');
        return;
    }
    
    if (!alumno.escuelaId || isNaN(alumno.escuelaId)) {
        mostrarError('Debes seleccionar una escuela');
        return;
    }
    
    console.log('Datos a enviar:', alumno); // Para debug
    
    try {
        let response;
        
        if (id) {
            // Actualizar
            alumno.id = parseInt(id);
            response = await fetch(`${API_URL}/alumnos/${id}`, {
                method: 'PUT',
                headers: { 
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(alumno)
            });
        } else {
            // Crear
            response = await fetch(`${API_URL}/alumnos`, {
                method: 'POST',
                headers: { 
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(alumno)
            });
        }
        
        // Leer la respuesta UNA SOLA VEZ
        const contentType = response.headers.get('content-type');
        let responseData;
        
        if (contentType && contentType.includes('application/json')) {
            responseData = await response.json();
        } else {
            responseData = await response.text();
        }
        
        console.log('Respuesta del servidor:', responseData); // Para debug
        
        if (response.ok) {
            mostrarExito(id ? 'Alumno actualizado correctamente' : 'Alumno creado correctamente');
            limpiarFormAlumno();
            cargarAlumnos();
            cargarListadoCompleto();
        } else {
            // Manejar error
            let errorMsg = 'Error al guardar el alumno';
            
            if (typeof responseData === 'object' && responseData.message) {
                errorMsg = responseData.message;
            } else if (typeof responseData === 'object' && responseData.title) {
                errorMsg = responseData.title;
            } else if (typeof responseData === 'string') {
                errorMsg = responseData;
            }
            
            console.error('Error del servidor:', responseData);
            mostrarError(errorMsg);
        }
        
    } catch (error) {
        console.error('Error de conexión:', error);
        mostrarError('Error de conexión con el servidor');
    }
});

async function editarAlumno(id) {
    try {
        const response = await fetch(`${API_URL}/alumnos/${id}`);
        const alumno = await response.json();

        document.getElementById('alumnoId').value = alumno.id;
        document.getElementById('alumnoNombre').value = alumno.nombre;
        document.getElementById('alumnoApellido').value = alumno.apellido;

        //Formatear fecha para el input type="date"
        const fecha = new Date(alumno.fechaNacimiento);
        const fechaFormateada = fecha.toISOString().split('T')[0];
        document.getElementById('alumnoFechaNacimiento').value = fechaFormateada;

        document.getElementById('alumnoGrado').value = alumno.grado || '';
        document.getElementById('alumnoPadreId').value = alumno.padreId;
        document.getElementById('alumnoMadreId').value = alumno.madreId;
        document.getElementById('alumnoEscuelaId').value = alumno.escuelaId;

        document.getElementById('tituloFormAlumno').innerHTML = 
            '<i class="bi bi-pencil"></i> Editar Alumno';

        document.getElementById('formAlumno').scrollIntoView({ behavior: 'smooth' });

    } catch (error) {
        console.error('Error: ', error);
        mostrarError('Error al cargar el alumno');
    }
}

async function eliminarAlumno(id) {
    if (!confirm('¿Estás seguro de eliminar este alumno?')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_URL}/alumnos/${id}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            mostrarExito('Alumno eliminado correctamente');
            cargarAlumnos();
            cargarListadoCompleto(); // Actualizar también el listado completo
        } else {
            const error = await response.json();
            mostrarError(error.message || 'Error al eliminar el alumno');
        }
        
    } catch (error) {
        console.error('Error:', error);
        mostrarError('Error al eliminar el alumno');
    }
}

function limpiarFormAlumno() {
    document.getElementById('formAlumno').reset();
    document.getElementById('alumnoId').value = '';
    document.getElementById('tituloFormAlumno').innerHTML = 
        '<i class="bi bi-plus-circle"></i> Nuevo Alumno';
}

// ============================================
// INICIALIZACIÓN
// ============================================

// Cargar el listado completo al iniciar
document.addEventListener('DOMContentLoaded', () => {
    cargarListadoCompleto();
});
