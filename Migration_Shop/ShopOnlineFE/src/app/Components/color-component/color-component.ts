import { Component } from '@angular/core';
import { Color } from '../../Models/color.model';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ColorService } from '../../Services/Color.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-color-component',
  imports: [CommonModule,FormsModule,ReactiveFormsModule,],
  templateUrl: './color-component.html',
  styleUrl: './color-component.css'
})
export class ColorComponent {
  colors: Color[] = [];
  colorForm: FormGroup;
  editingId: number | null = null;

  constructor(private service: ColorService, private fb: FormBuilder) {
    this.colorForm = this.fb.group({
      color1: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.loadColors();
  }

  loadColors(): void {
    this.service.getAll().subscribe(data => this.colors = data);
  }

  submit(): void {
    const color: Color = { color1: this.colorForm.value.color1, colorId: this.editingId ?? this.colors.length+1 };

    if (this.editingId === null) {
      this.service.create(color).subscribe(() => {
        this.colorForm.reset();
        this.loadColors();
      });
    } else {
      this.service.update(color).subscribe(() => {
        this.colorForm.reset();
        this.editingId = null;
        this.loadColors();
      });
    }
  }

  edit(color: Color): void {
    this.editingId = color.colorId;
    this.colorForm.patchValue({ color1: color.color1 });
  }

  delete(id: number): void {
    if (confirm("Are you sure to delete this color?")) {
      this.service.delete(id).subscribe((res) =>
        {
          //console.log(res);
          this.loadColors();
        }
      );
    }
  }

  cancelEdit(): void {
    this.editingId = null;
    this.colorForm.reset();
  }
}
