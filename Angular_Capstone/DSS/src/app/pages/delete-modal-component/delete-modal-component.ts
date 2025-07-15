import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-delete-modal-component',
  imports: [],
  templateUrl: './delete-modal-component.html',
  styleUrl: './delete-modal-component.css'
})
export class DeleteModalComponent {
  @Input() message: string = 'Are you sure ?';
  @Output() confirmed = new EventEmitter<boolean>();

  confirm() {
    this.confirmed.emit(true);
  }

  cancel() {
    this.confirmed.emit(false);
  }
}
