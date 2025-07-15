import { Component } from '@angular/core';
import { DocumentDetailsResponseDto } from '../../models/documentDetailsResponseDto';
import { DocumentAccessService } from '../../services/documentAccess.service';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { DocumentService } from '../../services/document.service';
import { getFileTypeIcon } from '../../utility/getFileTypeIcon';

@Component({
  selector: 'app-document-shared-component',
  imports: [RouterModule,CommonModule],
  templateUrl: './document-shared-component.html',
  styleUrl: './document-shared-component.css'
})
export class DocumentSharedComponent {
  getFileTypeIcon = getFileTypeIcon;
  documents: DocumentDetailsResponseDto[] | null = null;

  constructor(
    private documentAccessService:DocumentAccessService,
    private documentService:DocumentService,
    private route:Router
  ){}

  ngOnInit():void{
    this.documentAccessService.sharedFiles$.subscribe(doc =>{
      this.documents = doc;
    })

    this.documentAccessService.GetDocumentShared().subscribe();
  }

  onDetails(dto:DocumentDetailsResponseDto){
    this.documentService.DownloadSharedDocument(dto.fileName,dto.uploaderEmail).subscribe();
    this.route.navigate(['/main/document',dto.fileName]);
  }
}
