import { Component } from '@angular/core';
import { DocumentService } from '../../services/document.service';
import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';
import { getFileTypeIcon } from '../../utility/getFileTypeIcon';
import { Router, RouterModule } from '@angular/router';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { DocumentViewerService } from '../../services/documentView.service';
import { DeleteModalComponent } from '../delete-modal-component/delete-modal-component';
import { Toast } from 'bootstrap';

@Component({
  selector: 'app-all-documents-component',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule,RouterModule,DeleteModalComponent],
  templateUrl: './all-documents-component.html',
  styleUrl: './all-documents-component.css'
})
export class AllDocumentsComponent {
  getFileTypeIcon = getFileTypeIcon;
  documents: DocumentDetailsResponseDto[] | null = null;
  filterForm!: FormGroup;
  ascending: boolean = true;
  sortBy: string = '';
  showFilterSidebar: boolean = false;
  showDeleteModal = false;

  constructor(private fb: FormBuilder, private documentService: DocumentService,private route:Router,
    private documentAccesService:DocumentAccessService,
    private documentViewerService:DocumentViewerService
  ) {}

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      userEmail: [''],
      filename: [''],
      sortBy: [''],
      ascending: [true]
    });

    this.documentService.allDocuments$.subscribe(doc => {
      this.documents = doc;
    });

    const values = this.filterForm.value;
    this.documentService
      .GetAllDocumentDetails(values.userEmail, values.filename, values.sortBy, values.ascending)
      .subscribe();

    this.filterForm.valueChanges.pipe(debounceTime(500)).subscribe(values => {
      this.documentService
        .GetAllDocumentDetails(values.userEmail, values.filename, values.sortBy, values.ascending)
        .subscribe();
    });
  }

  clearFilters() {
    this.filterForm.setValue({
      userEmail: '',
      filename: '',
      sortBy: '',
      ascending: true
    });
    this.ascending = true;
    this.sortBy = '';
  }

  toggleSortDirection() {
    this.ascending = !this.ascending;
    this.filterForm.get('ascending')?.setValue(this.ascending);
  }

  toggleSidebar() {
    this.showFilterSidebar = !this.showFilterSidebar;
  }
  clearControl(controlName: string): void {
    this.filterForm.get(controlName)?.setValue('');
  }

  onDelete(dto:DocumentDetailsResponseDto){
    this.documentService.AdminDeleteDocument(dto.fileName,dto.uploaderEmail).subscribe({
      next:(res) =>{
        console.log(res);
        this.showToast("File Deleted Succefully","danger");
      },
      error:(err)=>{
        console.error(err);
        this.showToast(err.error.error.errorMessage,"danger");
      }
    })
  }

  // openDeleteConfirm(){
  //   this.showDeleteModal = true;
  // }

  // handleDeleteConfirm(result: boolean,dto:DocumentDetailsResponseDto) {
  //   this.showDeleteModal = false;
  //   console.log(dto);
  //   if (result) {
  //     console.log(dto);
  //     this.onDelete(dto);
  //   }
  // }

  onDetails(dto:DocumentDetailsResponseDto){
    this.documentService.DownloadSharedDocument(dto.fileName,dto.uploaderEmail).subscribe((res:any)=>{
      console.log(res);
    });
    this.documentAccesService.GetSharedUsersAdmin(dto.fileName,dto.uploaderEmail).subscribe();
    this.documentViewerService.GetViewerofFileAdmin(dto.fileName,dto.uploaderEmail).subscribe();
    this.route.navigate(['/main/documentadmin',dto.fileName]);
  }

  showToast(message: string, type: 'success' | 'danger') {
    const toastEl = document.getElementById('liveToast');
    const toastBody = document.querySelector('.toast-body');

    toastBody!.textContent = message;
    toastEl!.classList.remove('bg-success', 'bg-danger');
    toastEl!.classList.add(type === 'success' ? 'bg-success' : 'bg-danger');

    const toast = new Toast(toastEl!);
    toast.show();
  }
}
