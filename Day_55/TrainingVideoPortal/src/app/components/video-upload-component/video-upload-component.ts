import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { VideoService } from '../../services/video.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-video-upload-component',
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './video-upload-component.html',
  styleUrl: './video-upload-component.css'
})
export class VideoUploadComponent {
  uploadForm: FormGroup;
  uploadSuccess = false;
  uploadError = false;

  constructor(
    private fb: FormBuilder,
    private videoService: VideoService
  ) {
    this.uploadForm = this.fb.group({
      title: ['', Validators.required],
      description: ['', Validators.required],
      file: [null, Validators.required]
    });
  }

  onFileChange(event: any) {
    const file = event.target.files[0];
    this.uploadForm.patchValue({ file: file });
  }

  onSubmit() {
    if (this.uploadForm.invalid) return;

    const formData = new FormData();
    formData.append('title', this.uploadForm.value.title);
    formData.append('description', this.uploadForm.value.description);
    formData.append('file', this.uploadForm.value.file);

    this.videoService.uploadVideo(formData).subscribe({
      next: () => {
        this.uploadSuccess = true;
        this.uploadError = false;
        this.uploadForm.reset();
      },
      error: (err) => {
        console.log(err);
        this.uploadSuccess = false;
        this.uploadError = true;
      }
    });
  }
}
