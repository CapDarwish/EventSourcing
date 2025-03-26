// Example: src/app/employments/add/page.tsx
'use client';

import React from 'react';
import AddEmploymentForm from '@/components/AddEmpoymentForm';


export default function AddEmploymentPage() {
  const handleAssociationComplete = () => {
    console.log('Employment association complete!');
    // You might want to refresh a list of employments or navigate
    alert('Role Assigned Successfully!');
  };

  return (
    <main className="min-h-screen bg-gray-50 dark:bg-gray-900 py-12 transition-colors duration-200 ease-in-out">
      <AddEmploymentForm onAssociationComplete={handleAssociationComplete} />
    </main>
  );
}

