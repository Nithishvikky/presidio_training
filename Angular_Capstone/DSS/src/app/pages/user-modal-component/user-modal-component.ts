import { Component, EventEmitter, Input, Output } from '@angular/core';
import bootstrap from 'bootstrap';
import { UserResponseDto } from '../../models/userResponseDto';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-modal-component',
  imports: [CommonModule],
  templateUrl: './user-modal-component.html',
  styleUrl: './user-modal-component.css'
})
export class UserModalComponent {
  @Input() user!: UserResponseDto;
  @Input() show: boolean = false;
  @Output() closed = new EventEmitter<void>();

  closeModal() {
    this.show = false;
    this.closed.emit();
  }
}
