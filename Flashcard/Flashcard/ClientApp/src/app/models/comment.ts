import { User } from '../models/user';

/**
 * Comment interface
 */
export interface Comment {
    id: number;
    cardId: number,
    userId: string,
    user: User,
    comment_text: string
    comment_time: Date
}
