// src/components/AddPersonForm.tsx
'use client';

import React, { useState, FormEvent, useEffect } from 'react';
import type { Person } from './PersonList'; // Import the Person type

// Update command might only need Name if Id is in the URL, adjust as needed
interface UpdatePersonCommand {
  id: string;
  name: string;

}

// Create command remains the same
interface CreatePersonCommand {
  id: string;
  name: string;
}

interface AddPersonFormProps {
  personToEdit: Person | null; // Person object if editing, null if adding
  onUpdateComplete: () => void; // Callback after add/update success
  onCancelEdit: () => void; // Callback to cancel editing mode
}

const AddPersonForm: React.FC<AddPersonFormProps> = ({
  personToEdit,
  onUpdateComplete,
  onCancelEdit,
}) => {
  const [name, setName] = useState<string>('');
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const apiUrl = process.env.API_BASE_URL;
  const isEditing = personToEdit !== null;

  // Effect to populate form when personToEdit changes
  useEffect(() => {
    if (isEditing) {
      setName(personToEdit.name);
      setError(null); // Clear errors when starting edit
      setSuccessMessage(null); // Clear success messages
    } else {
      // Optionally clear form when switching back to add mode (e.g., after cancelling)
      // setName(''); // Uncomment if you want the form to clear when cancelling edit
    }
  }, [personToEdit, isEditing]); // Depend on personToEdit

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsLoading(true);
    setError(null);
    setSuccessMessage(null);

    if (!name.trim()) {
      setError('Name cannot be empty.');
      setIsLoading(false);
      return;
    }

    try {
      if (isEditing) {
        // --- UPDATE Logic ---
        const updateData: UpdatePersonCommand = {
          id: personToEdit.id,
          name: name.trim(),
        };
        console.log(`Updating person ${personToEdit.id}:`, updateData);

        // --- !!! Replace with your actual UPDATE API call (e.g., PUT/PATCH) !!! ---
        const response = await fetch(`${apiUrl}/api/persons/${personToEdit.id}`, { // Example URL
          method: 'PUT', // or 'PATCH'
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(updateData),
        });
        if (!response.ok) {
           let errorMessage = `HTTP error! status: ${response.status}`;
           try { const errorBody = await response.json(); errorMessage = errorBody.message || errorBody.title || errorMessage; } catch (e) {}
           throw new Error(errorMessage);
        }

        setSuccessMessage(`Person "${updateData.name}" updated successfully!`);
        onUpdateComplete(); // Notify parent component

      } else {
        // --- CREATE Logic (Existing) ---
        const newId = crypto.randomUUID();
        const createData: CreatePersonCommand = {
          id: newId,
          name: name.trim(),
        };
        console.log('Creating new person:', createData);

        // --- !!! Replace with your actual CREATE API call (e.g., POST) !!! ---
        const response = await fetch(apiUrl + '/api/persons', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(createData),
        });
         if (!response.ok) {
           let errorMessage = `HTTP error! status: ${response.status}`;
           try { const errorBody = await response.json(); errorMessage = errorBody.message || errorBody.title || errorMessage; } catch (e) {}
           throw new Error(errorMessage);
        }
        console.log('API Create Success');
        setSuccessMessage(`Person "${createData.name}" added successfully!`);
        setName(''); // Clear form only on successful creation
        onUpdateComplete(); // Notify parent component
      }
    } catch (err) {
      console.error(`Failed to ${isEditing ? 'update' : 'add'} person:`, err);
      setError(
        err instanceof Error ? err.message : 'An unknown error occurred.',
      );
    } finally {
      setIsLoading(false);
    }
  };

  const handleCancel = () => {
    setError(null);
    setSuccessMessage(null);
    // Maybe clear name field? Depends on desired UX
    // setName(personToEdit?.Name || ''); // Reset to original name if needed
    setName(''); // Or just clear it
    onCancelEdit(); // Notify parent
  };

  return (
    <div className="max-w-md mx-auto p-6 bg-white dark:bg-gray-800 rounded-lg shadow-xl dark:shadow-2xl border border-gray-200 dark:border-gray-700">
      <h2 className="text-2xl font-semibold text-gray-800 dark:text-gray-100 mb-6 text-center">
        {isEditing ? 'Edit Person' : 'Add New Person'}
      </h2>
      <form onSubmit={handleSubmit} noValidate>
        <div className="mb-4">
          <label
            htmlFor="name"
            className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1"
          >
            Name
          </label>
          <input
            type="text"
            id="name"
            value={name}
            onChange={(e) => {
              setName(e.target.value);
              if (error) setError(null);
              if (successMessage) setSuccessMessage(null);
            }}
            placeholder="Enter person's name"
            required
            className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md shadow-sm bg-white dark:bg-gray-700 text-gray-900 dark:text-gray-100 placeholder-gray-400 dark:placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-indigo-500 dark:focus:ring-indigo-400 focus:border-indigo-500 dark:focus:border-indigo-400 transition duration-150 ease-in-out disabled:bg-gray-100 dark:disabled:bg-gray-600 disabled:cursor-not-allowed"
            disabled={isLoading}
          />
        </div>

        {error && (
          <div
            className="mb-4 p-3 bg-red-100 dark:bg-red-900/30 border border-red-400 dark:border-red-600 text-red-700 dark:text-red-300 rounded-md text-sm"
            role="alert"
          >
            <p>
              <span className="font-medium">Error:</span> {error}
            </p>
          </div>
        )}

        {successMessage && (
          <div
            className="mb-4 p-3 bg-green-100 dark:bg-green-900/30 border border-green-400 dark:border-green-600 text-green-700 dark:text-green-300 rounded-md text-sm"
            role="alert"
          >
            <p>
              <span className="font-medium">Success:</span> {successMessage}
            </p>
          </div>
        )}

        <div className="mt-6 flex flex-col sm:flex-row sm:space-x-3 space-y-3 sm:space-y-0">
          <button
            type="submit"
            className={`w-full sm:w-auto flex-grow py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white focus:outline-none focus:ring-2 focus:ring-offset-2 dark:focus:ring-offset-gray-800 transition duration-150 ease-in-out ${
              isLoading
                ? 'bg-indigo-400 dark:bg-indigo-700/60 cursor-not-allowed'
                : 'bg-indigo-600 hover:bg-indigo-700 dark:bg-indigo-500 dark:hover:bg-indigo-600 focus:ring-indigo-500 dark:focus:ring-indigo-400'
            }`}
            disabled={isLoading}
          >
            {isLoading
              ? isEditing
                ? 'Updating...'
                : 'Adding...'
              : isEditing
              ? 'Update Person'
              : 'Add Person'}
          </button>
          {isEditing && ( // Show Cancel button only when editing
            <button
              type="button" // Important: type="button" to prevent form submission
              onClick={handleCancel}
              disabled={isLoading} // Disable cancel while submitting
              className="w-full sm:w-auto py-2 px-4 border border-gray-300 dark:border-gray-500 rounded-md shadow-sm text-sm font-medium text-gray-700 dark:text-gray-300 bg-white dark:bg-gray-700 hover:bg-gray-50 dark:hover:bg-gray-600 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 dark:focus:ring-offset-gray-800 dark:focus:ring-indigo-400 transition duration-150 ease-in-out disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Cancel
            </button>
          )}
        </div>
      </form>
    </div>
  );
};

export default AddPersonForm;

