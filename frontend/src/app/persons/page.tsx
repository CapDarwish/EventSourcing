// Example: src/app/some-page/page.tsx
'use client'; // This page now needs state, so it must be a Client Component

import React, { useState } from 'react';
import AddPersonForm from '@/components/AddPersonForm';
import PersonList, { Person } from '@/components/PersonList'; // Import Person type
import AddEmploymentForm from '@/components/AddEmpoymentForm';

export default function AddPersonPage() {
  // State to hold the person currently being edited
  const [editingPerson, setEditingPerson] = useState<Person | null>(null);
  // State to trigger list refresh
  const [listKey, setListKey] = useState<number>(0);

  // Handler for when Edit is clicked in the list
  const handleEdit = (person: Person) => {
    console.log('Editing person:', person);
    setEditingPerson(person);
    // Optionally scroll to the form or give focus
    // document.getElementById('name')?.focus(); // Example
  };


  const handleAssociationComplete = () => {
    console.log('Employment association complete!');
    // You might want to refresh a list of employments or navigate
    alert('Role Assigned Successfully!');
  };

  // Handler for when Add or Update is successfully completed in the form
  const handleUpdateComplete = () => {
    console.log('Update/Add complete. Refreshing list.');
    setEditingPerson(null); // Exit editing mode
    setListKey((prevKey) => prevKey + 1); // Increment key to trigger list refresh
  };

  // Handler for when Cancel is clicked in the form
  const handleCancelEdit = () => {
    console.log('Cancelled edit.');
    setEditingPerson(null); // Exit editing mode
  };

  return (
    <main className="min-h-screen bg-gray-50 dark:bg-gray-900 py-12 transition-colors duration-200 ease-in-out">
      <div className="space-y-8">
        {/* Pass editing state and handlers to the form */}
        <AddPersonForm
          personToEdit={editingPerson}
          onUpdateComplete={handleUpdateComplete}
          onCancelEdit={handleCancelEdit}
        />
        {/* Pass edit handler and refresh key to the list */}
        <PersonList onEdit={handleEdit} refreshKey={listKey} />

        <AddEmploymentForm onAssociationComplete={handleAssociationComplete} />

      </div>
    </main>
  );
}

