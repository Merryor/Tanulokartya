import { User } from '../models/user';

/**
 * Deck interface
 */
export interface Deck {
    id: number;
    name: string;
    content: string;
    module: number;
    deck_number: number;
    activated: boolean;
    status: number;
    applicationUserId: string;
    applicationUser: User
}
