import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryModule, GalleryItem, ImageItem } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-member-detail',
  standalone: true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule, TabsModule, GalleryModule, TimeagoModule],
})
export class MemberDetailComponent implements OnInit {
  member: Member | undefined;
  images: GalleryItem[] = [];

  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.loadMember();
  }

  loadMember() {
    const username = this.route.snapshot.paramMap.get('username');
    if (!username) return;
    this.memberService.getMember(username).subscribe({
      next: (response) => {
        this.member = response;
        this.getImages();
      },
    });
  }

  getImages() {
    if (!this.member) return;

    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({ src: photo.url, thumb: photo.url }));
    }
  }
}
