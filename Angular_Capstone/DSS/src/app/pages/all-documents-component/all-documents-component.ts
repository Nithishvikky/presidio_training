import { Component } from '@angular/core';
import { DocumentService } from '../../services/document.service';
import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { debounceTime } from 'rxjs';

@Component({
  selector: 'app-all-documents-component',
  imports: [CommonModule,FormsModule,ReactiveFormsModule],
  templateUrl: './all-documents-component.html',
  styleUrl: './all-documents-component.css'
})
export class AllDocumentsComponent {
  documents: DocumentDetailsResponseDto[] | null = null;
  filterForm!:FormGroup;
  ascending:boolean = true;
  sortBy:string = "";

  constructor(private fb:FormBuilder,private documentService:DocumentService){}

  ngOnInit():void{
    this.filterForm = this.fb.group({
      userEmail:[''],
      filename: [''],
      sortBy: [''],
      ascending:[true]
    })

    this.documentService.allDocuments$.subscribe(doc =>{
      this.documents = doc;
      console.log(this.documents);
    })

    const values = this.filterForm.value;
    this.documentService
        .GetAllDocumentDetails(values.userEmail,values.filename,values.sortBy,values.ascending)
        .subscribe();

    this.filterForm.valueChanges
      .pipe(debounceTime(300))
      .subscribe(values => {
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

  onSortby(event:Event){
    const value = (event.currentTarget as HTMLElement).id;

    this.filterForm.get('sortBy')?.setValue(value);
    this.sortBy = value;


    this.ascending = !this.ascending;
    this.filterForm.get('ascending')?.setValue(this.ascending);

  }
}
