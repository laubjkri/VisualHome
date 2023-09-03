import { Component } from '@angular/core';
import { User } from 'src/app/models/user';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent {
  ActiveButtonEnum = ActiveButtonEnum; // TypeScript syntax for making an enum available as a class member
  public activeButton: ActiveButtonEnum = ActiveButtonEnum.None;
  ToggleAddUserButton() { this.activeButton === ActiveButtonEnum.AddUser ? this.activeButton = ActiveButtonEnum.None : this.activeButton = ActiveButtonEnum.AddUser; }
  ToggleRemoveUserButton() { this.activeButton === ActiveButtonEnum.RemoveUser ? this.activeButton = ActiveButtonEnum.None : this.activeButton = ActiveButtonEnum.RemoveUser; }
  ToggleChangeUserButton() { this.activeButton === ActiveButtonEnum.ChangeUser ? this.activeButton = ActiveButtonEnum.None : this.activeButton = ActiveButtonEnum.ChangeUser; }

  public newUser: User = new User;

  


}


export enum ActiveButtonEnum {
  None = 0,
  AddUser = 1,
  RemoveUser,
  ChangeUser
}

