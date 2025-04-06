import Draggable from 'react-draggable';
import Paper, { PaperProps } from '@mui/material/Paper';

export function DraggablePaperComponent(props: PaperProps) {
  return (
    <Draggable
      handle="#draggable-dialog-title"
      cancel={'[class*="MuiDialogContent-root"]'}
    >
      <Paper {...props} />
    </Draggable>
  );
}