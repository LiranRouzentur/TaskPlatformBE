/**
 * Task Management JavaScript Module
 * Handles all AJAX operations for task management
 */

// Toast notification system
function showToast(message, type = 'success') {
    const toastContainer = document.querySelector('.toast-container');
    const toastHtml = `
        <div class="toast align-items-center text-white bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);
    
    const newToast = toastContainer.lastElementChild;
    const bsToast = new bootstrap.Toast(newToast, {
        autohide: true,
        delay: 5000
    });
    bsToast.show();
    
    // Remove toast element after it's hidden
    newToast.addEventListener('hidden.bs.toast', function() {
        newToast.remove();
    });
}

// Add new task to the table
function addTaskToTable(taskData) {
    const tableBody = document.querySelector('.task-table tbody');
    if (!tableBody) {
        return;
    }
    
    // Remove any "no tasks" message
    const noTasksMessage = document.querySelector('.no-tasks');
    if (noTasksMessage) {
        noTasksMessage.remove();
    }
    
    const newRow = createTaskRow(taskData);
    // Insert at the beginning to maintain descending ID order
    tableBody.insertBefore(newRow, tableBody.firstChild);
}

// Create a task row element
function createTaskRow(taskData) {
    const row = document.createElement('tr');
    row.setAttribute('data-task-id', taskData.id);
    
    // Set row class for closed tasks
    if (taskData.status === 0) {
        row.classList.add('closed');
    }
    
    // ID cell
    const idCell = document.createElement('td');
    idCell.textContent = taskData.id;
    
    // Task info cell
    const taskCell = document.createElement('td');
    const currentStatus = taskData.taskType?.statuses?.find(s => s.statusId === taskData.status);
    const totalStages = taskData.taskType?.statuses?.length ?? 0;
    const statusText = taskData.status === 0 
        ? 'Completed' 
        : `${currentStatus?.name ?? `Status ${taskData.status}`} (${taskData.status}/${totalStages})`;
    
    taskCell.innerHTML = `
        <div class="task-info">
            <div class="task-type">
                <strong>Type:</strong> ${taskData.taskType?.name ?? `Type ${taskData.typeId}`}
            </div>
            <div class="task-status">
                <strong>Status:</strong> <span>${statusText}</span>
            </div>
        </div>
    `;
    
    // Users info cell
    const usersCell = document.createElement('td');
    usersCell.innerHTML = `
        <div class="users-info">
            <div class="assigned-user">
                <strong>Assigned:</strong> ${taskData.user?.name ?? `User ${taskData.userId}`}
            </div>
            <div class="next-user">
                <strong>Next:</strong> ${taskData.nextAssignedUser?.name ?? `User ${taskData.nextAssignedUserId}`}
            </div>
        </div>
    `;
    
    // Requirement cell
    const requirementCell = document.createElement('td');
    if (taskData.status === 0) {
        requirementCell.innerHTML = '<span class="no-requirement">Task closed</span>';
    } else {
        const nextStatus = taskData.taskType?.statuses?.find(s => s.statusId === taskData.status + 1);
        if (nextStatus?.requirement) {
            requirementCell.innerHTML = `
                <div class="requirement-section">
                    <div class="requirement-title">
                        <strong>${nextStatus.requirementDescription || nextStatus.requirement}</strong>
                    </div>
                    <div class="requirement-input">
                        <textarea 
                            name="requirement_${taskData.id}"
                            placeholder="Enter your input here..."
                            class="requirement-field"
                            rows="3"></textarea>
                    </div>
                </div>
            `;
        } else if (nextStatus?.isFinal) {
            requirementCell.innerHTML = '<span class="no-requirement">Ready to close</span>';
        } else {
            requirementCell.innerHTML = '<span class="no-requirement">No requirement needed</span>';
        }
    }
    
    // Actions cell
    const actionsCell = document.createElement('td');
    actionsCell.className = 'actions';
    if (taskData.status === 0) {
        actionsCell.innerHTML = '<div class="action-buttons"><button type="button" class="btn btn-danger btn-sm" onclick="deleteTask(' + taskData.id + ')">Delete</button></div>';
    } else {
        const currentStatus = taskData.taskType?.statuses?.find(s => s.statusId === taskData.status);
        if (currentStatus?.isFinal) {
            actionsCell.innerHTML = `
                <div class="action-buttons">
                    <button type="button" class="btn btn-success btn-sm" onclick="closeTask(${taskData.id})">Close</button>
                    ${taskData.status > 1 ? `<button type="button" class="btn btn-warning btn-sm" onclick="reverseTask(${taskData.id})">Reverse</button>` : ''}
                    <button type="button" class="btn btn-danger btn-sm" onclick="deleteTask(${taskData.id})">Delete</button>
                </div>
            `;
        } else {
            actionsCell.innerHTML = `
                <div class="action-buttons">
                    <button type="button" class="btn btn-primary btn-sm" onclick="advanceTask(${taskData.id})">Advance</button>
                    ${taskData.status > 1 ? `<button type="button" class="btn btn-warning btn-sm" onclick="reverseTask(${taskData.id})">Reverse</button>` : ''}
                    <button type="button" class="btn btn-danger btn-sm" onclick="deleteTask(${taskData.id})">Delete</button>
                </div>
            `;
        }
    }
    
    // Append all cells to the row
    row.appendChild(idCell);
    row.appendChild(taskCell);
    row.appendChild(usersCell);
    row.appendChild(requirementCell);
    row.appendChild(actionsCell);
    
    return row;
}

// Update task row in the DOM
function updateTaskRow(taskData) {
    console.log('Updating task row with data:', taskData);
    
    // Find the row by task ID using a more reliable selector
    const row = document.querySelector(`tr[data-task-id="${taskData.id}"]`);
    if (!row) {
        console.error('Task row not found for ID:', taskData.id);
        return;
    }
    
    // Update task info
    const taskCell = row.querySelector('td:nth-child(2)');
    if (taskCell) {
        const currentStatus = taskData.taskType?.statuses?.find(s => s.statusId === taskData.status);
        const totalStages = taskData.taskType?.statuses?.length ?? 0;
        console.log('Current status:', currentStatus, 'Total stages:', totalStages, 'Task status:', taskData.status);
        
        const statusText = taskData.status === 0 
            ? 'Completed' 
            : `${currentStatus?.name ?? `Status ${taskData.status}`} (${taskData.status}/${totalStages})`;
        
        taskCell.innerHTML = `
            <div class="task-info">
                <div class="task-type">
                    <strong>Type:</strong> ${taskData.taskType?.name ?? `Type ${taskData.typeId}`}
                </div>
                <div class="task-status">
                    <strong>Status:</strong> <span>${statusText}</span>
                </div>
            </div>
        `;
    }
    
    // Update users info
    const usersCell = row.querySelector('td:nth-child(3)');
    if (usersCell) {
        usersCell.innerHTML = `
            <div class="users-info">
                <div class="assigned-user">
                    <strong>Assigned:</strong> ${taskData.user?.name ?? `User ${taskData.userId}`}
                </div>
                <div class="next-user">
                    <strong>Next:</strong> ${taskData.nextAssignedUser?.name ?? `User ${taskData.nextAssignedUserId}`}
                </div>
            </div>
        `;
    }
    
    // Update requirement section
    const requirementCell = row.querySelector('td:nth-child(4)');
    if (requirementCell) {
        if (taskData.status === 0) {
            requirementCell.innerHTML = '<span class="no-requirement">Task closed</span>';
        } else {
            const nextStatus = taskData.taskType?.statuses?.find(s => s.statusId === taskData.status + 1);
            if (nextStatus?.requirement) {
                requirementCell.innerHTML = `
                    <div class="requirement-section">
                        <div class="requirement-title">
                            <strong>${nextStatus.requirementDescription || nextStatus.requirement}</strong>
                        </div>
                        <div class="requirement-input">
                            <textarea 
                                name="requirement_${taskData.id}"
                                placeholder="Enter your input here..."
                                class="requirement-field"
                                rows="3"></textarea>
                        </div>
                    </div>
                `;
            } else if (nextStatus?.isFinal) {
                requirementCell.innerHTML = '<span class="no-requirement">Ready to close</span>';
            } else {
                requirementCell.innerHTML = '<span class="no-requirement">No requirement needed</span>';
            }
        }
    }
    
    // Update action buttons
    const actionsCell = row.querySelector('.actions');
    if (actionsCell) {
        if (taskData.status === 0) {
            actionsCell.innerHTML = '<div class="action-buttons"><button type="button" class="btn btn-danger btn-sm" onclick="deleteTask(' + taskData.id + ')">Delete</button></div>';
        } else {
            const currentStatus = taskData.taskType?.statuses?.find(s => s.statusId === taskData.status);
            if (currentStatus?.isFinal) {
                actionsCell.innerHTML = `
                    <div class="action-buttons">
                        <button type="button" class="btn btn-success btn-sm" onclick="closeTask(${taskData.id})">Close</button>
                        ${taskData.status > 1 ? `<button type="button" class="btn btn-warning btn-sm" onclick="reverseTask(${taskData.id})">Reverse</button>` : ''}
                        <button type="button" class="btn btn-danger btn-sm" onclick="deleteTask(${taskData.id})">Delete</button>
                    </div>
                `;
            } else {
                actionsCell.innerHTML = `
                    <div class="action-buttons">
                        <button type="button" class="btn btn-primary btn-sm" onclick="advanceTask(${taskData.id})">Advance</button>
                        ${taskData.status > 1 ? `<button type="button" class="btn btn-warning btn-sm" onclick="reverseTask(${taskData.id})">Reverse</button>` : ''}
                        <button type="button" class="btn btn-danger btn-sm" onclick="deleteTask(${taskData.id})">Delete</button>
                    </div>
                `;
            }
        }
    }
    
    // Update row class for closed tasks
    if (taskData.status === 0) {
        row.classList.add('closed');
    } else {
        row.classList.remove('closed');
    }
}

// Task creation
function createTask() {
    const taskType = document.getElementById('taskType').value;
    const userId = document.getElementById('user').value;
    
    if (!taskType || !userId) {
        showToast('Please select both task type and user', 'danger');
        return;
    }
    
    fetch('/MvcTasks/CreateTask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            TypeId: parseInt(taskType),
            UserId: parseInt(userId)
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showToast(data.message || 'Task created successfully');
            // Clear the form
            document.getElementById('taskType').value = '';
            document.getElementById('user').value = '';
            
            // Add the new task to the UI
            if (data.task) {
                addTaskToTable(data.task);
            }
        } else {
            showToast(data.message || 'Failed to create task', 'danger');
        }
    })
    .catch(error => {
        showToast('An error occurred while creating the task', 'danger');
    });
}

// Task advancement
function advanceTask(taskId) {
    const requirementField = document.querySelector(`textarea[name="requirement_${taskId}"]`);
    const requirement = requirementField ? requirementField.value.trim() : '';
    
    // Show loading state
    const advanceBtn = document.querySelector(`button[onclick="advanceTask(${taskId})"]`);
    const originalText = advanceBtn.textContent;
    advanceBtn.textContent = 'Advancing...';
    advanceBtn.disabled = true;
    
    fetch('/MvcTasks/AdvanceTask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            TaskId: taskId,
            Requirement: requirement
        })
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(data => {
        if (data.success) {
            showToast(data.message || 'Task advanced successfully');
            // Clear the requirement field
            if (requirementField) {
                requirementField.value = '';
            }
            
            // Update the UI with the new task data
            if (data.task) {
                updateTaskRow(data.task);
            }
        } else {
            showToast(data.message || 'Failed to advance task', 'danger');
        }
    })
    .catch(error => {
        console.error('Error advancing task:', error);
        showToast('An error occurred while advancing the task', 'danger');
    })
    .finally(() => {
        // Restore button state
        advanceBtn.textContent = originalText;
        advanceBtn.disabled = false;
    });
}

// Task reversal
function reverseTask(taskId) {
    fetch('/MvcTasks/ReverseTask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            TaskId: taskId
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showToast(data.message || 'Task reversed successfully');
            
            // Update the UI with the new task data
            if (data.task) {
                updateTaskRow(data.task);
            }
            
            // If there's a reversed requirement, populate the textarea
            if (data.reversedRequirement) {
                const requirementField = document.querySelector(`textarea[name="requirement_${data.reversedTaskId}"]`);
                if (requirementField) {
                    requirementField.value = data.reversedRequirement;
                }
            }
        } else {
            showToast(data.message || 'Failed to reverse task', 'danger');
        }
    })
    .catch(error => {
        showToast('An error occurred while reversing the task', 'danger');
    });
}

// Task closure
function closeTask(taskId) {
    fetch('/MvcTasks/CloseTask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            TaskId: taskId
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showToast(data.message || 'Task closed successfully');
            
            // Update the UI with the new task data
            if (data.task) {
                updateTaskRow(data.task);
            }
        } else {
            showToast(data.message || 'Failed to close task', 'danger');
        }
    })
    .catch(error => {
        showToast('An error occurred while closing the task', 'danger');
    });
}

// Task deletion
function deleteTask(taskId) {
    if (!confirm('Are you sure you want to delete this task?')) {
        return;
    }
    
    fetch('/MvcTasks/DeleteTask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
            TaskId: taskId
        })
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            showToast(data.message || 'Task deleted successfully');
            // Remove the row from the table
            const row = document.querySelector(`tr[data-task-id="${taskId}"]`);
            if (row) {
                row.remove();
            }
            // If no rows left, show the no-tasks message
            const tableBody = document.querySelector('.task-table tbody');
            if (tableBody && tableBody.children.length === 0) {
                const container = document.querySelector('.task-table-container');
                if (container) {
                    container.innerHTML += '<div class="no-tasks"><p>No tasks available. Create a new task to get started.</p></div>';
                }
            }
        } else {
            showToast(data.message || 'Failed to delete task', 'danger');
        }
    })
    .catch(error => {
        showToast('An error occurred while deleting the task', 'danger');
    });
}

// Filter tasks by user
function filterTasks() {
    const userId = document.getElementById('userFilter').value;
    const currentUrl = new URL(window.location);
    
    if (userId) {
        currentUrl.searchParams.set('userId', userId);
    } else {
        currentUrl.searchParams.delete('userId');
    }
    
    window.location.href = currentUrl.toString();
}

// Initialize toasts on page load
document.addEventListener('DOMContentLoaded', function() {
    // Initialize and show toasts
    const toasts = document.querySelectorAll('.toast');
    toasts.forEach(function(toast) {
        const bsToast = new bootstrap.Toast(toast, {
            autohide: true,
            delay: 5000
        });
        bsToast.show();
    });
}); 