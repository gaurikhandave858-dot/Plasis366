const CLASSES = {
  'Draft': 'st-draft',
  'Submitted': 'st-submitted',
  'Under Review': 'st-review',
  'Design In Progress': 'st-progress',
  'Design Submitted': 'st-design',
  'Customer Review': 'st-customer',
  'Approved': 'st-approved',
  'Revision Requested': 'st-revision',
  'Completed': 'st-completed',
}

function StatusBadge({ status }) {
  return <span className={`pd-badge ${CLASSES[status] ?? 'st-draft'}`}>{status}</span>
}

export default StatusBadge